using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Sunday.App_Start;
using Sunday.Models;

namespace Sunday.Controllers
{
    /// <summary>
    /// 用户 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Users")]
    public class UserController : ApiController
    {
        // NLog
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private ApplicationDbContext context = new ApplicationDbContext();

        #region 帮助程序

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [Route("GetUserInfo")]
        public TempSysUser GetUserInfo()
        {
            // 通过token获取当前用户
            var userId = User.Identity.GetUserId();

            // 处理逻辑
            var oldModel = (from t in context.Users
                            where t.IsDeleted == false && t.Id == userId

                            join r in context.SysRoles on t.RoleId equals r.Id
                            into JoinedRole
                            from r in JoinedRole.DefaultIfEmpty()

                            join d in context.SysDepts on t.DeptId equals d.Id
                            into JoinedDept
                            from d in JoinedDept.DefaultIfEmpty()

                            select new
                            {
                                t.Id,
                                t.RegisterType,
                                t.UserType,
                                t.RealName,
                                t.NickName,
                                t.DeptId,
                                d.DeptName,
                                t.RoleId,
                                r.RoleName,
                                r.Menus,
                                t.IsDeleted,
                                t.IsEnable,
                                t.CreateDate
                            }).FirstOrDefault();

            var tempSysUser = new TempSysUser
            {
                Id = oldModel.Id,
                RegisterType = oldModel.RegisterType,
                UserType = oldModel.UserType,
                RealName = oldModel.RealName,
                NickName = oldModel.NickName,
                DeptId = oldModel.DeptId,
                DeptName = oldModel.DeptName,
                RoleId = oldModel.RoleId,
                RoleName = oldModel.RoleName,
                Menus = oldModel.Menus,
                IsEnable = oldModel.IsEnable,
                IsDeleted = oldModel.IsDeleted,
                CreateDate = oldModel.CreateDate != null ? Convert.ToDateTime(oldModel.CreateDate).ToString("yyyy-MM-dd HH:mm:ss") : ""
            };

            // 处理返回值
            return tempSysUser;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("PostUser")]
        public async Task<IHttpActionResult> PostUser(RegisterBindingModel model)
        {
            var user = new ApplicationUser()
            {
                UserType = model.UserType,
                UserName = model.Account,
                RegisterType = "0",
                NickName = model.NickName,
                RealName = model.RealName,
                DeptId = model.DeptId,
                RoleId = model.RoleId,
                PasswordHash = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreateDate = DateTime.Now,
                IsEnable = true,
                IsDeleted = false
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("添加用户：{0}", JsonConvert.SerializeObject(user));

            return Content<ApplicationUser>(HttpStatusCode.Created, user);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("PutUser")]
        public async Task<IHttpActionResult> PutUser(RegisterBindingModel model)
        {
            var oldUser = await UserManager.FindByIdAsync(model.Id);

            oldUser.Id = model.Id;
            oldUser.NickName = model.NickName;
            oldUser.RealName = model.RealName;
            oldUser.DeptId = model.DeptId;
            oldUser.RoleId = model.RoleId;
            oldUser.Email = model.Email;
            oldUser.PhoneNumber = model.PhoneNumber;
            oldUser.UpdateDate = DateTime.Now;
            oldUser.UpdateUserId = User.Identity.GetUserId();

            IdentityResult result = await UserManager.UpdateAsync(oldUser);

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("添加用户：{0}", JsonConvert.SerializeObject(oldUser));

            return Content<ApplicationUser>(HttpStatusCode.OK, oldUser);
        }

        /// <summary>
        /// 添加用户（通过手机号）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("PostUserByPhone")]
        public async Task<IHttpActionResult> PostUserByPhone(RegisterPhoneBindingModel model)
        {
            var user = new ApplicationUser()
            {

            };

            IdentityResult result = await UserManager.CreateAsync(user, "123456");

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("添加用户：{0}", JsonConvert.SerializeObject(user));

            return Content<ApplicationUser>(HttpStatusCode.Created, user);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Id"></param>
        [Route("Delete")]
        public async Task<IHttpActionResult> Delete(string Id)
        {
            var oldUser = await UserManager.FindByIdAsync(Id);

            // 处理逻辑
            oldUser.IsDeleted = true;
            oldUser.DeleteDate = DateTime.Now;
            oldUser.DeleteUserId = User.Identity.GetUserId();
            IdentityResult result = await UserManager.UpdateAsync(oldUser);

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("添加用户：{0}", JsonConvert.SerializeObject(oldUser));

            return Content<ApplicationUser>(HttpStatusCode.OK, oldUser);
        }

        ///// <summary>
        ///// 批量删除用户
        ///// </summary>
        ///// <param name="idList"></param>
        //[Route("Delete")]
        //public async Task<string> DeleteBatch(string idList)
        //{

        //    // =====================实际测试这样写并不能回滚================
        //    using (var tran = context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // 通过token获取当前用户
        //            var userId = User.Identity.GetUserId();

        //            // 处理逻辑
        //            string[] ids = idList.Split(',');
        //            foreach (var item in ids)
        //            {
        //                var oldUser = await UserManager.FindByIdAsync(item);
        //                oldUser.IsDeleted = true;
        //                oldUser.DeleteDate = DateTime.Now;
        //                oldUser.DeleteUserId = User.Identity.GetUserId();
        //                IdentityResult result = await UserManager.UpdateAsync(oldUser);
        //            }
                    
        //            // 必须调用commit，不然不会保存数据
        //            tran.Commit();

        //            // 记录日志
        //            logger.Warn("批量删除用户：{0}", idList);

        //            return idList;
        //        }
        //        catch (Exception ex)
        //        {
        //            // 出错回滚
        //            tran.Rollback();

        //            // 记录日志
        //            logger.Error(ex, "批量删除用户失败！{0}", idList);
        //            throw;
        //        }

        //    }
        //}

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PostChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return Content<object>(HttpStatusCode.BadRequest, ModelState);
            }

            var userId = User.Identity.GetUserId();
            IdentityUser user = await UserManager.FindByIdAsync(userId);

            IdentityResult result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("修改用户密码：{0}", JsonConvert.SerializeObject(user));

            return Content<ChangePasswordBindingModel>(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// 设置用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return Content<object>(HttpStatusCode.BadRequest, ModelState);
            }

            var userId = User.Identity.GetUserId();
            IdentityUser user = await UserManager.FindByIdAsync(userId);

            IdentityResult result = await UserManager.AddPasswordAsync(userId, model.NewPassword);

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("设置用户密码：{0}", JsonConvert.SerializeObject(user));

            return Content<SetPasswordBindingModel>(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// 重置用户密码（默认密码：123456）
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(string Id)
        {
            IdentityUser user = await UserManager.FindByIdAsync(Id);

            string token = await UserManager.GeneratePasswordResetTokenAsync(Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(Id, token, "123456");

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("修改用户密码：{0}", JsonConvert.SerializeObject(user));

            return Content<string>(HttpStatusCode.OK, Id);
        }

        /// <summary>
        /// 重置用户密码（默认密码：123456）
        /// </summary>
        /// <returns></returns>
        [Route("ResetPasswordByToken")]
        public async Task<IHttpActionResult> ResetPasswordByToken()
        {
            var userId = User.Identity.GetUserId();
            IdentityUser user = await UserManager.FindByIdAsync(userId);

            string token = await UserManager.GeneratePasswordResetTokenAsync(userId);
            IdentityResult result = await UserManager.ResetPasswordAsync(userId, token, "123456");

            if (!result.Succeeded)
            {
                return Content<IdentityResult>(HttpStatusCode.BadRequest, result);
            }

            // 记录日志
            logger.Info("重置用户密码：{0}", JsonConvert.SerializeObject(user));

            return Content<string>(HttpStatusCode.OK, userId);
        }

        //[Route("Logout")]
        //public IHttpActionResult Logout()
        //{
        //    Authentication.SignOut();
        //    //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
        //    //Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return Ok("ok");
        //}
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class TempSysUser
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 注册方式 0:后台分配 1:手机号注册
        /// </summary>
        public string RegisterType { get; set; }

        /// <summary>
        /// 用户类型 0：鲁中 1：经销商 2：客户
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 是否可用的
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否删除的
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public string Menus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
    }
}