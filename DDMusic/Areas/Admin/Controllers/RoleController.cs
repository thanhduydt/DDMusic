using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole>roleManager,UserManager<UserModel> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //Lấy thông tin các Role từ csdl
          ViewBag.listRole = await _roleManager.Roles.ToListAsync();
            return View();
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>CreateRole([Bind("Id,Name")] RoleModel roleModel)
        {
            //Kiểm tra tên Role có tồn tại
         var result= await  _roleManager.FindByNameAsync(roleModel.Name);
            if (result != null)
            {
                ViewBag.eName = "Tên Role đã tồn tại.";
                return View(roleModel);
            }
            else
            {
                //Tạo mới 1 Role
                var role = new IdentityRole(roleModel.Name);
               await _roleManager.CreateAsync(role);
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            if(id!=null)
            {
                //Kiểm tra role có tồn tại
              var role=await _roleManager.FindByIdAsync(id);
                if(role!=null)
                {
                    await _roleManager.DeleteAsync(role);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UserRoleAsync()
        {
            //Lấy danh sách User
            List<UserModel> ListUser = await _userManager.Users.ToListAsync();
            //Tạo List chứa thông tin User và các Role của User. 
            List<UserRoleViewModel> ListUserRole = new List<UserRoleViewModel>();
            foreach(UserModel userModel in ListUser)
            {
                //Lấy các Role của User
                var roleUser = await _userManager.GetRolesAsync(userModel);
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel();
                userRoleViewModel.IdUser = userModel.Id;
                userRoleViewModel.NameUser = userModel.UserName;
                List<Role> listRole = new List<Role>();
                for (int i=0;i<roleUser.Count;i++)
                {
                    Role role = new Role();
                    role.NameRole = roleUser[i];
                    listRole.Add(role);
                }
                userRoleViewModel.ListRole = listRole;
                ListUserRole.Add(userRoleViewModel);
            }

         return View(ListUserRole);
        }
        public async Task<IActionResult> EditUserRole(string idUser)
        {
            //Lấy thông tin User
            var user = await _userManager.FindByIdAsync(idUser);
            //Gán thông tin User vào Model
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel();
            userRoleViewModel.IdUser = user.Id;
            userRoleViewModel.NameUser = user.UserName;
            List<Role> listRole = new List<Role>();
            foreach (var role in _roleManager.Roles)
            {
                var roleModel = new Role
                {
                    IdRole = role.Id,
                    NameRole = role.Name
                };
                //User có Role được kiểm tra.
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    roleModel.IsSelected = true;
                }
                else
                {
                    roleModel.IsSelected = false;
                }
                listRole.Add(roleModel);
            }
            userRoleViewModel.ListRole = listRole;
            return View(userRoleViewModel);
        }
        [HttpPost]
        public async Task<IActionResult>EditUserRole(UserRoleViewModel model)
        {
            //Lấy thông tin User
            var user = await _userManager.FindByIdAsync(model.IdUser);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            //Lấy thông tin Role của User
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in model.ListRole)
            {
                //Thêm và xóa role của User
                if (roles.Contains(item.NameRole) == false && item.IsSelected == true)
                {
                    await _userManager.AddToRoleAsync(user, item.NameRole);
                }
                else if (roles.Contains(item.NameRole) == true && item.IsSelected == false)
                {
                    await _userManager.RemoveFromRoleAsync(user, item.NameRole);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult>EditRole(string id)
        {
            //Lấy thông tin Role 
            IdentityRole role =await  _roleManager.FindByIdAsync(id);
            RoleModel roleModel = new RoleModel();
            roleModel.Id = role.Id.ToString();
            roleModel.Name = role.Name;
            
            return View(roleModel);
        }
        [HttpPost]
        public async Task<IActionResult>EditRole(RoleModel model)
        {
            if(model.Name!=null)
            {
                if (_roleManager.FindByNameAsync(model.Name).Result == null)
                {
                    var role = await _roleManager.FindByIdAsync(model.Id);
                    role.Name = model.Name;
                    var r = await _roleManager.UpdateAsync(role);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.eName = "Tên Role đã tồn tại.";
            return View(model);
        }
    }
}
