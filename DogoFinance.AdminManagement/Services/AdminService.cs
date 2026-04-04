using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.Extensions.Logging;

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

                var (hash, salt) = HashHelper.CreateHash(request.Password);

                var user = new TblUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
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

        public async Task<ApiResponse> GetAdmins()
        {
            var admins = await BaseRepository().FindList<TblUser>(u => u.IsSystemUser);
            return new ApiResponse { Success = true, Data = admins, Message = "Admins retrieved" };
        }
    }
}
