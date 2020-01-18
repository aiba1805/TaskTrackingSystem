using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TTS.BLL;
using TTS.DAL.Entities;
using TTS.Shared.Models;

namespace TTS.Web.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper, UserService userService)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userService = userService;
        }

        public IActionResult Index() => View((from role in _roleManager.Roles.ToList() select _mapper.Map<RoleViewModel>(role)).ToList());
        

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model.Name);
            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(model.Name));
            if (result.Succeeded) return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model.Name);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<RoleViewModel>(role));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _roleManager.UpdateAsync(_mapper.Map<IdentityRole<Guid>>(model));
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangeUserRole(Guid? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUser((Guid)userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userService.GetUserRoles(user);
            var model = new ChangeUserRoleViewModel()
            {
                UserId = user.Id,
                UserRoles = userRoles,
                AllRoles = (from role in _roleManager.Roles select _mapper.Map<RoleViewModel>(role)).ToList()
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(Guid userId, List<string> roles)
        {
            var user = await _userService.GetUser(userId);
            if (user == null) return NotFound();
            var userRoles = await _userService.GetUserRoles(user);
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            var addRes = await _userService.AddRolesAsync(user, addedRoles);
            var remRes = await _userService.RemoveRolesAsync(user, removedRoles);
            if(addRes.Succeeded && remRes.Succeeded) return RedirectToAction("Index","Users");
            foreach (var error in addRes.Errors)
            {
                ModelState.AddModelError(error.Code,error.Description);
            }
            foreach (var error in remRes.Errors)
            {
                ModelState.AddModelError(error.Code,error.Description);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<RoleViewModel>(role));
        }


        [HttpPost, ActionName("Delete"),ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("Index");
        }
    }
}