using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using Sunday.App_Start;
using System;

namespace Sunday.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加个人资料数据，若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        // 添加属性

        /// <summary>
        /// 注册方式 0:后台分配 1:手机号注册
        /// </summary>
        public string RegisterType { get; set; }

        /// <summary>
        /// 用户类型 0：总部 1：经销商 2：客户
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
        /// 部门
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUserId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 删除者
        /// </summary>
        public string DeleteUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteDate { get; set; }

        /// <summary>
        /// 是否可用的
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否删除的
        /// </summary>
        public bool IsDeleted { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            // 在第一次启动网站时初始化数据库添加管理员用户凭据到数据库
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        // 部门表
        public IDbSet<SysDept> SysDepts { get; set; }
        // 角色表
        public IDbSet<SysRole> SysRoles { get; set; }
        // 日志表
        public IDbSet<SysLog> SysLogs { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}