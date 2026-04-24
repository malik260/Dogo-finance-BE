using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Services
{
    public class PortfolioService : DataRepository, IPortfolioService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(IUnitOfWork uow, ILogger<PortfolioService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetList()
        {
            var response = new ApiResponse();
            try
            {
                var entities = await _uow.Portfolios.GetPortfoliosDetailed();
                var result = new List<PortfolioDto>();

                foreach (var entity in entities)
                {
                    var dto = new PortfolioDto
                    {
                        PortfolioId = entity.PortfolioId,
                        Name = entity.Name,
                        Code = entity.Code,
                        PortfolioTypeId = entity.PortfolioTypeId,
                        PortfolioTypeName = entity.PortfolioType?.Name,
                        RiskLevel = entity.RiskLevel,
                        Description = entity.Description,
                        ExpectedAnnualReturn = entity.ExpectedAnnualReturn,
                        IsActive = entity.IsActive,
                        
                        LockInPeriodDays = entity.LockInPeriodDays,
                        MinHoldingPeriodDays = entity.MinHoldingPeriodDays,
                        ExitFeePercentage = entity.ExitFeePercentage,
                        NoticePeriodDays = entity.NoticePeriodDays,
                        ApprovalThresholdAmount = entity.ApprovalThresholdAmount,

                        Allocations = new List<PortfolioAllocationRuleDto>()
                    };

                    var rules = await _uow.Portfolios.GetAllocationRules(entity.PortfolioId);
                    var assetClasses = await _uow.Portfolios.GetAssetClasses();
                    var allInstruments = await _uow.Portfolios.GetInstruments();

                    foreach (var rule in rules)
                    {
                        var ruleDto = new PortfolioAllocationRuleDto
                        {
                            Id = rule.Id,
                            PortfolioId = rule.PortfolioId,
                            AssetClassId = rule.AssetClassId,
                            AssetClassName = assetClasses.FirstOrDefault(a => a.AssetClassId == rule.AssetClassId)?.Name,
                            TargetPercentage = rule.TargetPercentage,
                            MinPercentage = rule.MinPercentage,
                            MaxPercentage = rule.MaxPercentage,
                            ExpectedReturn = rule.ExpectedReturn,
                            Instruments = new List<PortfolioInstrumentDto>()
                        };

                        var instruments = (await _uow.Portfolios.GetPortfolioInstruments(entity.PortfolioId))
                                          .Where(i => i.AssetClassId == rule.AssetClassId);
                        
                        foreach (var inst in instruments)
                        {
                            ruleDto.Instruments.Add(new PortfolioInstrumentDto
                            {
                                Id = inst.Id,
                                PortfolioId = inst.PortfolioId,
                                AssetClassId = inst.AssetClassId,
                                InstrumentId = inst.InstrumentId,
                                InstrumentName = allInstruments.FirstOrDefault(i => i.InstrumentId == inst.InstrumentId)?.Name,
                                Percentage = inst.TargetWeight
                            });
                        }

                        dto.Allocations.Add(ruleDto);
                    }

                    result.Add(dto);
                }

                response.SetMessage("Retrieved portfolios with full hierarchy", true, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed portfolios");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Save(PortfolioDto model)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.BeginTransactionAsync();

                var entity = model.PortfolioId == 0 ? new TblPortfolio() : await _uow.Portfolios.GetPortfolioById(model.PortfolioId);
                if (entity == null) { response.SetError("Not found", 404); return response; }

                entity.Name = model.Name;
                entity.Code = model.Code;
                entity.PortfolioTypeId = model.PortfolioTypeId;
                entity.RiskLevel = model.RiskLevel;
                entity.Description = model.Description;
                entity.ExpectedAnnualReturn = model.ExpectedAnnualReturn;
                entity.IsActive = model.IsActive;

                entity.LockInPeriodDays = model.LockInPeriodDays;
                entity.MinHoldingPeriodDays = model.MinHoldingPeriodDays;
                entity.ExitFeePercentage = model.ExitFeePercentage;
                entity.NoticePeriodDays = model.NoticePeriodDays;
                entity.ApprovalThresholdAmount = model.ApprovalThresholdAmount;
                
                if (model.PortfolioId == 0) entity.CreatedAt = DateTime.UtcNow;

                await _uow.Portfolios.SavePortfolio(entity);
                int portfolioId = entity.PortfolioId;

                // Handle Allocations
                if (model.Allocations != null)
                {
                    // 1. Get existing rules to identify deletions
                    var existingRules = await _uow.Portfolios.GetAllocationRules(portfolioId);
                    var incomingRuleIds = model.Allocations.Where(a => a.Id > 0).Select(a => a.Id).ToList();
                    
                    foreach (var existingRule in existingRules)
                    {
                        if (!incomingRuleIds.Contains(existingRule.Id))
                        {
                            // Delete instruments for this rule first
                            var instrumentsToDelete = await _uow.Portfolios.GetPortfolioInstruments(portfolioId);
                            foreach (var inst in instrumentsToDelete.Where(i => i.AssetClassId == existingRule.AssetClassId))
                            {
                                await _uow.Portfolios.DeletePortfolioInstrument(inst.Id);
                            }
                            await _uow.Portfolios.DeleteAllocationRule(existingRule.Id);
                        }
                    }

                    // 2. Save/Update incoming rules
                    foreach (var ruleDto in model.Allocations)
                    {
                        var ruleEntity = ruleDto.Id == 0 ? new TblPortfolioAllocationRule() : await _uow.Portfolios.GetAllocationRuleById(ruleDto.Id);
                        if (ruleEntity == null) continue;

                        ruleEntity.PortfolioId = portfolioId;
                        ruleEntity.AssetClassId = ruleDto.AssetClassId;
                        ruleEntity.TargetPercentage = ruleDto.TargetPercentage;
                        ruleEntity.MinPercentage = ruleDto.MinPercentage;
                        ruleEntity.MaxPercentage = ruleDto.MaxPercentage;
                        ruleEntity.ExpectedReturn = ruleDto.ExpectedReturn;

                        await _uow.Portfolios.SaveAllocationRule(ruleEntity);

                        // Handle Instruments for this Asset Class
                        if (ruleDto.Instruments != null)
                        {
                            var existingPIs = (await _uow.Portfolios.GetPortfolioInstruments(portfolioId))
                                             .Where(i => i.AssetClassId == ruleDto.AssetClassId).ToList();
                            
                            var incomingPIIds = ruleDto.Instruments.Where(i => i.Id > 0).Select(i => i.Id).ToList();

                            foreach (var epi in existingPIs)
                            {
                                if (!incomingPIIds.Contains(epi.Id)) await _uow.Portfolios.DeletePortfolioInstrument(epi.Id);
                            }

                            foreach (var piDto in ruleDto.Instruments)
                            {
                                var piEntity = piDto.Id == 0 ? new TblPortfolioInstrument() : await _uow.Portfolios.GetPortfolioInstrumentById(piDto.Id);
                                if (piEntity == null) continue;

                                piEntity.PortfolioId = portfolioId;
                                piEntity.AssetClassId = ruleDto.AssetClassId;
                                piEntity.InstrumentId = piDto.InstrumentId;
                                piEntity.TargetWeight = piDto.Percentage;

                                await _uow.Portfolios.SavePortfolioInstrument(piEntity);
                            }
                        }
                    }
                }

                await _uow.CommitAsync();
                response.SetMessage("Portfolio and allocations saved successfully", true);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error saving portfolio hierarchy");
                response.SetError("Internal server error", 500);
            }
            return response;
        }

        public async Task<ApiResponse> Delete(int id)
        {
            var response = new ApiResponse();
            try
            {
                await _uow.Portfolios.DeletePortfolio(id);
                response.SetMessage("Deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio");
                response.SetError("Internal server error", 500);
            }
            return response;
        }
    }
}
