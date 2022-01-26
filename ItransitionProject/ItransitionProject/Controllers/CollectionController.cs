using ItransitionProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ItransitionProject.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _applicationContext;
        private readonly IWebHostEnvironment _appEnvironment;

        public CollectionController(UserManager<User> userManager, ApplicationContext applicationContext, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
            _appEnvironment = webHostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> CreateDefaultCollection(string? userId)
        {
            if (UserCheck(userId))
            {
                var userCollection = new UserCollection()
                {
                    Name = "Default Collection",
                    Descritpion = "Collection Description",
                    CollectionType = CollectionType.Alcohol,
                    UserId = userId,
                    FileId = null
                };
                await _applicationContext.AddAsync(userCollection);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("UserPage", "Home", new { userId = userId });
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> AddProperty(CollectionProperty collectionProperty)
        {
            var collection = _applicationContext.UserCollections.Find(collectionProperty.CollectionId);
            if (UserCheck(collection.UserId))
            {
                await _applicationContext.AddAsync(collectionProperty);
                await _applicationContext.SaveChangesAsync();
                UpdateItemProperties(collectionProperty);
                return RedirectToAction("EditCollection", "Collection", new { collectionId = collectionProperty.CollectionId });
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> AddItem(UserItem userItem, List<string> tags)
        {
            var userCollection = _applicationContext.UserCollections.Find(userItem.CollectionId);
            if (UserCheck(userCollection.UserId))
            {
                await _applicationContext.AddAsync(userItem);
                if (userItem.ItemProperties != null)
                {
                    foreach (var property in userItem.ItemProperties)
                    {
                        property.UserItemId = userItem.Id;
                        await _applicationContext.AddAsync(property);
                    }
                }
                await _applicationContext.SaveChangesAsync();
                await AddTags(userItem.Id, tags);
                return RedirectToAction("EditCollection", "Collection", new { collectionId = userItem.CollectionId });
            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditCollectionItem(int? itemId)
        {
            var item = _applicationContext.UserItems.Find(itemId);
            if (item != null)
            {
                var collection = _applicationContext.UserCollections.Find(item.CollectionId);
                if (UserCheck(collection.UserId))
                {
                    EditItemViewModel editItem = new EditItemViewModel(_applicationContext, item);
                    return View(editItem);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditItem(EditItem editItem, List<string> tags)
        {
            var item = _applicationContext.UserItems.Find(editItem.Id);
            if (item != null)
            {
                var collection = _applicationContext.UserCollections.Find(item.CollectionId);
                if (UserCheck(collection.UserId))
                {
                    item.Name = editItem.Name;
                    _applicationContext.UserItems.Update(item);
                    _applicationContext.ItemProperties.UpdateRange(editItem.ItemProperties);
                    var oldTags = _applicationContext.ItemTags.Where(x => x.ItemId == item.Id).ToList();
                    _applicationContext.RemoveRange(oldTags);
                    await _applicationContext.SaveChangesAsync();
                    await AddTags(item.Id, tags);
                    return RedirectToAction("EditCollection", "Collection", new { collectionId = item.CollectionId });
                }
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> DeleteItem(int? itemId)
        {
            var item = _applicationContext.UserItems.Find(itemId);
            if (item != null)
            {
                var collection = _applicationContext.UserCollections.Find(item.CollectionId);
                if (UserCheck(collection.UserId))
                {
                    _applicationContext.Remove(item);
                    await _applicationContext.SaveChangesAsync();
                }
                return RedirectToAction("EditCollection", "Collection", new { collectionId = collection.Id });
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> RemoveProperty(int? propertyId)
        {
            var property = _applicationContext.CollectionProperties.Find(propertyId);
            if (property != null)
            {
                var collection = _applicationContext.UserCollections.Find(property.CollectionId);
                if (UserCheck(collection.UserId))
                {
                    var itemProperties = _applicationContext.ItemProperties.Where(p => p.ColletctionPropertyId == property.Id);
                    _applicationContext.ItemProperties.RemoveRange(itemProperties);
                    _applicationContext.CollectionProperties.Remove(property);
                    await _applicationContext.SaveChangesAsync();
                }
                return RedirectToAction("EditCollection", "Collection", new { collectionId = collection.Id });
            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditCollection(int? collectionId)
        {
            var collection = _applicationContext.UserCollections.Find(collectionId);
            if (collection != null)
            {
                if (UserCheck(collection.UserId))
                {
                    EditCollectionViewModel model = new EditCollectionViewModel(_applicationContext, collection);
                    ViewBag.Tags = _applicationContext.UserTags.ToList();
                    return View(model);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditCollection(EditCollection editCollection, IFormFile inputCollectionImage)
        {
            var collection = _applicationContext.UserCollections.Find(editCollection.Id);
            if (collection != null)
            {
                if (UserCheck(collection.UserId))
                {
                    collection.Name = editCollection.Name;
                    collection.Descritpion = editCollection.Description;
                    collection.CollectionType = editCollection.CollectionType;
                    _applicationContext.UserCollections.Update(collection);
                    if (editCollection.EditCollectionProperties != null)
                    {
                        foreach (var property in editCollection.EditCollectionProperties)
                        {
                            var p = _applicationContext.CollectionProperties.Find(property.Id);
                            p.Name = property.Name;
                            p.Property = property.Property;
                            _applicationContext.CollectionProperties.Update(p);
                        }
                    }
                    collection.FileModel = await AddFile(inputCollectionImage);
                    await _applicationContext.SaveChangesAsync();
                    return RedirectToAction("EditCollection", "Collection", new { collectionId = collection.Id });
                }
            }
            return BadRequest();
        }

        public async Task<IActionResult> CollectionPage(int? collectionId)
        {
            var collection = _applicationContext.UserCollections.Find(collectionId);
            if (collection != null)
            {
                CollectionPageViewModel collectionViewModel = new CollectionPageViewModel(_applicationContext, collection);
                return View(collectionViewModel);
            }
            return BadRequest();
        }

        public async Task<IActionResult> ItemPage(int? itemId)
        {
            var item = _applicationContext.UserItems.Find(itemId);
            if (item != null)
            {
                ItemViewModel itemView = new ItemViewModel(_applicationContext, item);
                return View(itemView);
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> AddComment(AddCommentView commentView)
        {
            Comment userComment = new Comment()
            {
                ItemId = commentView.ItemId,
                UserComment = commentView.Comment,
                User = await _userManager.FindByEmailAsync(User.Identity.Name)
            };
            await _applicationContext.AddAsync(userComment);
            await _applicationContext.SaveChangesAsync();
            await NofitySubscribers(commentView.ItemId);
            return RedirectToAction("ItemPage", "Collection", new { itemId = userComment.ItemId });
        }

        [Authorize]
        public async Task<IActionResult> LikeItem(int? itemId)
        {
            User user = await _userManager.FindByEmailAsync(User.Identity.Name);
            Like like = _applicationContext.Likes.FirstOrDefault(i => i.ItemId == itemId && i.User == user);
            _ = like != null ? _applicationContext.Remove(like) : await _applicationContext.AddAsync(new Like() { ItemId = itemId, User = user });
            await _applicationContext.SaveChangesAsync();
            return RedirectToAction("ItemPage", "Collection", new { itemId = itemId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteCollection(int? collectionId)
        {
            var collection = _applicationContext.UserCollections.Find(collectionId);
            if (collection != null)
            {
                string user = collection.UserId;
                if (UserCheck(collection.UserId))
                {
                    _applicationContext.UserCollections.Remove(collection);
                    await _applicationContext.SaveChangesAsync();
                }
                return RedirectToAction("UserPage", "Home", new { userId = user });
            }
            return BadRequest();
        }

        [Authorize]
        public async Task<FileModel> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                //string path = "/Files/" + User.Identity.Name + DateTime.Now.ToString("MM.dd.yyyy.HH.mm.ss") + uploadedFile.FileName;
                //using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                //{
                //    await uploadedFile.CopyToAsync(fileStream);
                //}
                using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                {

                    FileModel file = new FileModel { Name = uploadedFile.FileName, Data = binaryReader.ReadBytes((int)uploadedFile.Length) };
                    await _applicationContext.AddAsync(file);
                    await _applicationContext.SaveChangesAsync();
                    return file;
                }   
            }
            return null;
        }

        public async Task AddTags(int itemId, List<string> tags)
        {
            var item = _applicationContext.UserItems.Find(itemId);
            {
                if (item != null)
                {
                    foreach (var tag in tags)
                    {
                        UserTag userTag = _applicationContext.UserTags.FirstOrDefault(t => t.Tag == tag);
                        if (userTag == null)
                        {
                            UserTag newTag = new UserTag() { Tag = tag };
                            await _applicationContext.AddAsync(newTag);
                            await _applicationContext.AddAsync(new ItemTag() { ItemId = itemId, UserTag = newTag });
                        }
                        else
                        {
                            await _applicationContext.AddAsync(new ItemTag() { ItemId = itemId, UserTag = userTag });
                        }
                    }
                    await _applicationContext.SaveChangesAsync();
                }
            }
        }

        public async Task<IActionResult> Items(int? tagId)
        {
            List<ItemTag> items = _applicationContext.ItemTags.Include(i => i.UserItem).Where(t => t.TagId == tagId).ToList();
            return View(items);
        }

        public async Task UpdateItemProperties(CollectionProperty collectionProperty)
        {
            var items = _applicationContext.UserItems.Where(x => x.CollectionId == collectionProperty.CollectionId);
            foreach (var item in items)
            {
                var propery = new ItemProperty()
                {
                    UserItemId = item.Id,
                    ColletctionPropertyId = collectionProperty.Id,
                };
                propery.Value = (collectionProperty.Property == PropertyType.checkbox) ? "false" : "";
                await _applicationContext.AddAsync(propery);
            }
            await _applicationContext.SaveChangesAsync();
        }

        public bool UserCheck(string? userId)
        {
            User user = _applicationContext.Users.Find(userId);
            if (User.IsInRole("admin") || User.Identity.Name == user.Email)
            {
                return true;
            }
            return false;
        }

        public async Task NofitySubscribers(int itemId)
        {
            if (WebSocketController.Subscribers.ContainsKey(itemId))
            {
                foreach (var subscriber in WebSocketController.Subscribers[itemId])
                {
                    var serverMsg = Encoding.UTF8.GetBytes(itemId.ToString());
                    await subscriber.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}