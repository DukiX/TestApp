using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestApp.DB;
using TestApp.Enums;
using TestApp.ExceptionHandling;
using TestApp.Models;
using TestApp.Tools;

namespace TestApp.Services
{
    public interface IProductsService
    {
        Task<OutProizvodDTO> Add(InProizvodDTO model, HttpContext context);
        Task<OutProizvodDTO> Update(Guid id, InProizvodDTO model, HttpContext context);
        Task<List<OutProizvodDTO>> GetAll();
        Task<List<OutProizvodDTO>> GetAllForUser(HttpContext context);
        Task<OutProizvodDTO> Get(Guid id);
        Task<bool> Delete(Guid id);
        Task<MemoryStream> GetImage(Guid id);
        Task<bool> SaveImage(HttpContext context, Guid id);
        bool DeleteImage(Guid id);
    }

    public class ProductsService : IProductsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ProductsService(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<OutProizvodDTO> Add(InProizvodDTO model, HttpContext context)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            Guid id = Guid.NewGuid();

            _db.Proizvodi.Add(new Proizvod
            {
                Id = id,
                Naziv = model.Naziv,
                Cena = model.Cena,
                Opis = model.Opis,
                NacinKoriscenja = model.NacinKoriscenja,
                Prodavac = user
            });

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.DbError, "Greška pri čuvanju proizvoda u bazu podataka.");
            }

            return new OutProizvodDTO
            {
                Id = id,
                Naziv = model.Naziv,
                Cena = model.Cena,
                Opis = model.Opis,
                NacinKoriscenja = model.NacinKoriscenja,
                Prodavac = null
            };
        }

        public async Task<OutProizvodDTO> Update(Guid id, InProizvodDTO model, HttpContext context)
        {
            var proizvod = await _db.Proizvodi.Where(x => x.Id == id)?.Include(i => i.Prodavac).FirstOrDefaultAsync();
            if (proizvod == null)
                return null;

            proizvod.Naziv = model.Naziv;
            proizvod.Cena = model.Cena;
            proizvod.Opis = model.Opis;
            proizvod.NacinKoriscenja = model.NacinKoriscenja;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.DbError, "Greška pri čuvanju proizvoda u bazu podataka.");
            }

            return new OutProizvodDTO
            {
                Id = id,
                Naziv = model.Naziv,
                Cena = model.Cena,
                Opis = model.Opis,
                NacinKoriscenja = model.NacinKoriscenja,
                Prodavac = null
            };
        }

        public async Task<List<OutProizvodDTO>> GetAll()
        {
            var proizvodi = await _db.Proizvodi.Include(i => i.Prodavac).ToListAsync();

            List<OutProizvodDTO> outProizvodi = new List<OutProizvodDTO>();

            foreach (var proizvod in proizvodi)
            {
                string slika = null;

                try
                {
                    using var buffer = await GetImage(proizvod.Id);
                    slika = Convert.ToBase64String(buffer.GetBuffer());
                }
                catch (Exception) { }

                outProizvodi.Add(new OutProizvodDTO
                {
                    Id = proizvod.Id,
                    Naziv = proizvod.Naziv,
                    Cena = proizvod.Cena,
                    Opis = null,
                    NacinKoriscenja = proizvod.NacinKoriscenja,
                    Prodavac = null,
                    Slika = slika
                });
            }

            return outProizvodi;
        }

        public async Task<List<OutProizvodDTO>> GetAllForUser(HttpContext context)
        {
            string userId = TokensHelper.GetClaimFromJwt(context, CustomClaims.UserId.ToString());

            var proizvodi = await _db.Proizvodi.Include(i => i.Prodavac).Where(p => p.Prodavac.Id == userId).ToListAsync();

            List<OutProizvodDTO> outProizvodi = new List<OutProizvodDTO>();

            foreach (var proizvod in proizvodi)
            {
                string slika = null;

                try
                {
                    using var buffer = await GetImage(proizvod.Id);
                    slika = Convert.ToBase64String(buffer.GetBuffer());
                }
                catch (Exception) { }

                outProizvodi.Add(new OutProizvodDTO
                {
                    Id = proizvod.Id,
                    Naziv = proizvod.Naziv,
                    Cena = proizvod.Cena,
                    Opis = null,
                    NacinKoriscenja = proizvod.NacinKoriscenja,
                    Prodavac = null,
                    Slika = slika
                });
            }

            return outProizvodi;
        }

        public async Task<OutProizvodDTO> Get(Guid id)
        {
            var proizvod = await _db.Proizvodi.Where(x => x.Id == id)?.Include(i => i.Prodavac).FirstOrDefaultAsync();
            if (proizvod == null)
                throw new ErrorException(ErrorCode.ProductNotFound, "Proizvod nije pronađen.");

            Account acc = null;

            if (proizvod.Prodavac != null)
                acc = new Account
                {
                    Username = proizvod.Prodavac.UserName,
                    Email = proizvod.Prodavac.Email,
                    FirstName = proizvod.Prodavac.FirstName,
                    LastName = proizvod.Prodavac.LastName,
                    Address = proizvod.Prodavac.Address,
                    PhoneNumber = proizvod.Prodavac.PhoneNumber
                };

            string slika = null;

            try
            {
                using var buffer = await GetImage(id);
                slika = Convert.ToBase64String(buffer.GetBuffer());
            }
            catch (Exception) { }

            return new OutProizvodDTO
            {
                Id = proizvod.Id,
                Naziv = proizvod.Naziv,
                Cena = proizvod.Cena,
                Opis = proizvod.Opis,
                NacinKoriscenja = proizvod.NacinKoriscenja,
                Prodavac = acc,
                Slika = slika
            };
        }

        public async Task<bool> Delete(Guid id)
        {
            var proizvod = await _db.Proizvodi.FirstOrDefaultAsync(x => x.Id == id);
            if (proizvod == null)
                return false;

            try
            {
                var a = _db.Proizvodi.Remove(proizvod);

                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.DbError, "Greška pri brisanju proizvoda.");
            }

            try
            {
                DeleteImage(id);
            }
            catch (Exception) { }

            return true;
        }

        public async Task<MemoryStream> GetImage(Guid id)
        {
            try
            {
                var path1 = Path.Combine("Resources", "Images");
                var path = Path.Combine(path1, "Products");

                string imageName = id.ToString();

                var fileName = Path.Combine(path, imageName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return memory;
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
        }

        public async Task<bool> SaveImage(HttpContext context, Guid id)
        {
            IFormFile file;
            try
            {
                file = context.Request.Form.Files.FirstOrDefault(f => f.Name == "file");
                if (file == null)
                    throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
            if (file.Length > 10000000)
                throw new ErrorException(ErrorCode.ImageTooLarge, "Slika zauzima previše prostora.");

            var path1 = Path.Combine("Resources", "Images");
            var path = Path.Combine(path1, "Products");

            string imageName = id.ToString();

            var fullPath = Path.Combine(path, imageName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }

        public bool DeleteImage(Guid id)
        {
            try
            {
                var path1 = Path.Combine("Resources", "Images");
                var path = Path.Combine(path1, "Products");

                string imageName = id.ToString();

                var fullPath = Path.Combine(path, imageName);

                File.Delete(fullPath);

                return true;
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Greška pri brisanju slike.");
            }
        }
    }
}
