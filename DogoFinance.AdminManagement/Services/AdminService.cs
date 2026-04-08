using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.AdminManagement.Services
{
    public class AdminService : DataRepository, IAdminService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IUnitOfWork uow, ILogger<AdminService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> CreateAdmin(SignUpRequest request, int roleId)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var existingUser = await _uow.Users.GetByEmail(request.Email);
                if (existingUser != null)
                {
                    response.SetError("Email already in use.", 400);
                    return response;
                }

                var defaultPassword = "StaffPass1234!"; // As requested by USER
                var (hash, salt) = HashHelper.CreateHash(string.IsNullOrWhiteSpace(request.Password) ? defaultPassword : request.Password);

                var user = new TblUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = hash,
                    Salt = salt,
                    IsActive = true, // Admins created by super admin are active by default
                    IsLocked = false,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    IsSystemUser = true
                };

                await _uow.Users.SaveUser(user);

                // Assign Role
                var userRole = new TblUserRole
                {
                    UserId = user.UserId,
                    RoleId = roleId
                };
                await BaseRepository().Insert(userRole);

                await BaseRepository().CommitTrans();
                response.SetMessage("Admin user created successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "CreateAdmin Failed");
                response.SetError("Failed to create admin.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> UpdateAdmin(long userId, SignUpRequest request, int roleId)
        {
            var response = new ApiResponse();
            try
            {
                var user = await BaseRepository().FindEntity<TblUser>(u => u.UserId == userId);
                if (user == null)
                {
                    response.SetError("User not found.", 404);
                    return response;
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.ModifiedAt = DateTime.UtcNow;

                await BaseRepository().Update(user);

                // Update Role
                var userRole = await BaseRepository().FindEntity<TblUserRole>(ur => ur.UserId == userId);
                if (userRole != null)
                {
                    userRole.RoleId = roleId;
                    await BaseRepository().Update(userRole);
                }
                else
                {
                    userRole = new TblUserRole { UserId = userId, RoleId = roleId };
                    await BaseRepository().Insert(userRole);
                }

                response.SetMessage("User updated successfully.", 200);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAdmin Failed");
                response.SetError("Failed to update admin.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> GetAdmins()
        {
            var admins = await BaseRepository().AsQueryable<TblUser>(u => u.IsSystemUser)
                .Include(u => u.TblUserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var result = admins.Select(u => new {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.TblUserRoles.FirstOrDefault()?.Role?.Name ?? "Admin",
                IsActive = u.IsActive,
                IsLocked = u.IsLocked,
                CreatedAt = u.CreatedAt
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Admins retrieved" };
        }

        public async Task<ApiResponse> ListClients()
        {
            var customers = await BaseRepository().AsQueryable<TblCustomer>(c => true)
                .Include(c => c.User)
                .ToListAsync();
            var result = customers.Select(c => new {
                Id = "C" + c.CustomerId.ToString("D3"),
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.User?.Email ?? "N/A", // This depends on user being loaded
                Phone = c.PhoneNumber,
                Status = c.User?.IsActive == false ? "Locked" : (c.Ninverified == true && c.Ninverified == true ? "Active" : "Pending KYC"),
                KycLevel = c.Ninverified == true && c.Bvnverified == true ? "Level 3 - Verified" : (c.Bvnverified == true ? "Level 2 - Partial" : "Level 1 - Basic"),
                AccountBalance = 0, // In real life, calculate from wallet
                DateJoined = c.CreatedAt.ToString("MM/dd/yyyy")
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Clients retrieved" };
        }

        // --- ROLES ---
        public async Task<ApiResponse> GetRoles()
        {
            var roles = await BaseRepository().FindList<TblRole>(r => r.Id != 3);
            return new ApiResponse { Success = true, Data = roles, Message = "Roles retrieved" };
        }

        public async Task<ApiResponse> SaveRole(TblRole role)
        {
            if (role.Id > 0)
            {
                await BaseRepository().Update(role);
            }
            else
            {
                await BaseRepository().Insert(role);
            }
            return new ApiResponse { Success = true, Message = "Role saved successfully" };
        }

        public async Task<ApiResponse> DeleteRole(int id)
        {
            var role = await BaseRepository().FindEntity<TblRole>(r => r.Id == id);
            if (role == null) return new ApiResponse { Message = "Role not found", Status = 404 };
            await BaseRepository().Delete(role);
            return new ApiResponse { Success = true, Message = "Role deleted successfully" };
        }

        // --- ACCESS RIGHTS ---
        public async Task<ApiResponse> GetAccessRightsHierarchy(int roleId)
        {
            var modules = await BaseRepository().FindList<TblModule>(m => true);
            var allAccess = await BaseRepository().FindList<TblAccessRight>(ar => true);
            var roleAccess = (await BaseRepository().FindList<TblRoleAccessRight>(ra => ra.RoleId == roleId)).Select(ra => ra.AccessRightId).ToList();

            var hierarchy = modules.Select(m => new
            {
                m.Id,
                m.Name,
                m.Icon,
                m.Description,
                Permissions = allAccess.Where(ar => ar.ModuleId == m.Id).Select(ar => new
                {
                    ar.Id,
                    ar.Name,
                    ar.Label,
                    IsSelected = roleAccess.Contains(ar.Id)
                }).ToList()
            }).ToList();

            return new ApiResponse { Success = true, Data = hierarchy, Message = "Access mapping retrieved" };
        }

        public async Task<ApiResponse> UpdateRoleAccessRights(int roleId, List<int> accessRightIds)
        {
            var db = await BaseRepository().BeginTrans();
            try
            {
                // Clear existing
                var existing = await BaseRepository().FindList<TblRoleAccessRight>(ra => ra.RoleId == roleId);
                foreach (var ra in existing)
                {
                    await BaseRepository().Delete(ra);
                }

                // Add new
                foreach (var arId in accessRightIds)
                {
                    await BaseRepository().Insert(new TblRoleAccessRight { RoleId = roleId, AccessRightId = arId });
                }

                await BaseRepository().CommitTrans();
                return new ApiResponse { Success = true, Message = "Access rights updated" };
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "UpdateRoleAccessRights failed");
                return new ApiResponse { Message = "Failed to update access rights", Status = 500 };
            }
        }
    }
}
