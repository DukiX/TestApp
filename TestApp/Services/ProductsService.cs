using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        Task<bool> Add(InProizvodDTO model, HttpContext context);
        Task<List<OutProizvodDTO>> Get();
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

        public async Task<bool> Add(InProizvodDTO model, HttpContext context)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            _db.Proizvodi.Add(new Proizvod
            {
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
                throw new ErrorException(ErrorCode.UserNotFound, "Greška pri čuvanju proizvoda u bazu podataka.");
            }

            return true;
        }

        public async Task<List<OutProizvodDTO>> Get()
        {
            var proizvodi = await _db.Proizvodi.Include(i => i.Prodavac).ToListAsync();

            List<OutProizvodDTO> outProizvodi = new List<OutProizvodDTO>();

            foreach (var proizvod in proizvodi)
            {
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

                outProizvodi.Add(new OutProizvodDTO
                {
                    Naziv = proizvod.Naziv,
                    Cena = proizvod.Cena,
                    Opis = proizvod.Opis,
                    NacinKoriscenja = proizvod.NacinKoriscenja,
                    Prodavac = acc
                });
            }

            return outProizvodi;
        }
    }
}
