using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public interface IOrderService
    {
        Task<List<OutNarudzbinaDTO>> Add(InNarudzbinaDTO model, HttpContext context);
        Task<List<OutProdavacNarudzbinaDTO>> GetAll(HttpContext context);
        Task<OutProdavacNarudzbinaDTO> Update(Guid id, UpdateNarudzbinaDTO model, HttpContext context);
        Task<OutProdavacNarudzbinaDTO> Get(Guid id, HttpContext context);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<List<OutNarudzbinaDTO>> Add(InNarudzbinaDTO model, HttpContext context)
        {
            if (model == null || model.ListaElemenata == null || model.ListaElemenata.Count == 0)
                return null;

            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            var listaNarudzbina = new List<Narudzbina>();
            foreach (var el in model.ListaElemenata)
            {
                var proizvod = _db.Proizvodi.Where(p => p.Id == el.Id)?.Include(i => i.Prodavac).FirstOrDefault();
                if (proizvod == null)
                    continue;
                var narudzbinaZaOvogProdavca = listaNarudzbina.FirstOrDefault(n => n.Prodavac.Id == proizvod.Prodavac.Id);
                if (narudzbinaZaOvogProdavca == null)
                {
                    narudzbinaZaOvogProdavca = new Narudzbina
                    {
                        Id = Guid.NewGuid(),
                        ListaElemenata = new List<ElementKorpe>
                        {
                            new ElementKorpe
                            {
                                Id = Guid.NewGuid(),
                                Kolicina = el.Kolicina,
                                Proizvod = proizvod
                            }
                        },
                        Kupac = user,
                        StatusNarudzbine = StatusNarudzbine.Nova,
                        VremeIsporukeUDanima = null,
                        Prodavac = proizvod.Prodavac
                    };
                    listaNarudzbina.Add(narudzbinaZaOvogProdavca);
                }
                else
                {
                    narudzbinaZaOvogProdavca.ListaElemenata.Add(new ElementKorpe
                    {
                        Id = Guid.NewGuid(),
                        Kolicina = el.Kolicina,
                        Proizvod = proizvod
                    });
                }
            }

            foreach (var narudzbina in listaNarudzbina)
            {
                _db.Narudzbine.Add(narudzbina);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.DbError, "Greška pri čuvanju narudzbine u bazu podataka.");
            }

            var outListaNarudzbina = new List<OutNarudzbinaDTO>();
            foreach (var narudzbina in listaNarudzbina)
            {
                var outNar = new OutNarudzbinaDTO
                {
                    Id = narudzbina.Id,
                    Prodavac = new Account
                    {
                        FirstName = narudzbina.Prodavac.FirstName,
                        LastName = narudzbina.Prodavac.LastName,
                        Email = narudzbina.Prodavac.Email,
                        PhoneNumber = narudzbina.Prodavac.PhoneNumber
                    },
                    ListaElemenata = new List<OutElementKorpeDTO>()
                };
                foreach (var el in narudzbina.ListaElemenata)
                {
                    outNar.ListaElemenata.Add(new OutElementKorpeDTO
                    {
                        Kolicina = el.Kolicina,
                        Proizvod = new OutProizvodDTO
                        {
                            Id = el.Proizvod.Id,
                            Naziv = el.Proizvod.Naziv,
                            Cena = el.Proizvod.Cena,
                            Opis = el.Proizvod.Opis,
                            NacinKoriscenja = el.Proizvod.NacinKoriscenja,
                            Prodavac = null
                        }
                    });
                }
                outListaNarudzbina.Add(outNar);
            }

            return outListaNarudzbina;
        }

        public async Task<List<OutProdavacNarudzbinaDTO>> GetAll(HttpContext context)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            var narudzbine = await _db.Narudzbine.Include(n => n.Kupac).Include(n => n.Prodavac)
                .Include(n => n.ListaElemenata).ThenInclude(k => k.Proizvod).Where(k => k.Prodavac == user)?.ToListAsync();

            if (narudzbine == null)
                return null;

            List<OutProdavacNarudzbinaDTO> outProdavacNarudzbine = new List<OutProdavacNarudzbinaDTO>();

            foreach (var narudzbina in narudzbine)
            {
                var outProdavacNarudzbina = new OutProdavacNarudzbinaDTO
                {
                    Id = narudzbina.Id,
                    StatusNarudzbine = narudzbina.StatusNarudzbine,
                    VremeIsporukeUDanima = narudzbina.VremeIsporukeUDanima,
                    Kupac = new Account
                    {
                        Address = narudzbina.Kupac.Address,
                        Email = narudzbina.Kupac.Email,
                        FirstName = narudzbina.Kupac.FirstName,
                        LastName = narudzbina.Kupac.LastName,
                        PhoneNumber = narudzbina.Kupac.PhoneNumber
                    },
                    ListaElemenata = new List<OutElementKorpeDTO>()
                };
                foreach (var el in narudzbina.ListaElemenata)
                {
                    outProdavacNarudzbina.ListaElemenata.Add(new OutElementKorpeDTO
                    {
                        Kolicina = el.Kolicina,
                        Proizvod = new OutProizvodDTO
                        {
                            Id = el.Proizvod.Id,
                            Naziv = el.Proizvod.Naziv,
                            Cena = el.Proizvod.Cena,
                            Opis = el.Proizvod.Opis,
                            NacinKoriscenja = el.Proizvod.NacinKoriscenja,
                            Prodavac = null
                        }
                    });
                }
                outProdavacNarudzbine.Add(outProdavacNarudzbina);
            }

            return outProdavacNarudzbine;
        }

        public async Task<OutProdavacNarudzbinaDTO> Update(Guid id, UpdateNarudzbinaDTO model, HttpContext context)
        {
            var narudzbina = await _db.Narudzbine.Include(n => n.Kupac).Include(n => n.Prodavac)
                .Include(n => n.ListaElemenata).ThenInclude(k => k.Proizvod).FirstOrDefaultAsync(a => a.Id == id);

            if (narudzbina == null)
                return null;

            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            if (narudzbina.Prodavac.Id != user.Id)
                throw new ErrorException(ErrorCode.OrderAccessError, "Nemate pravo da pristupite ovoj narudzbini.");

            narudzbina.StatusNarudzbine = model.StatusNarudzbine;
            narudzbina.VremeIsporukeUDanima = model.VremeIsporukeUDanima;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.DbError, "Greška pri čuvanju narudzbine u bazu podataka.");
            }

            var outProdavacNarudzbina = new OutProdavacNarudzbinaDTO
            {
                Id = narudzbina.Id,
                StatusNarudzbine = narudzbina.StatusNarudzbine,
                VremeIsporukeUDanima = narudzbina.VremeIsporukeUDanima,
                Kupac = new Account
                {
                    Address = narudzbina.Kupac.Address,
                    Email = narudzbina.Kupac.Email,
                    FirstName = narudzbina.Kupac.FirstName,
                    LastName = narudzbina.Kupac.LastName,
                    PhoneNumber = narudzbina.Kupac.PhoneNumber
                },
                ListaElemenata = new List<OutElementKorpeDTO>()
            };
            foreach (var el in narudzbina.ListaElemenata)
            {
                outProdavacNarudzbina.ListaElemenata.Add(new OutElementKorpeDTO
                {
                    Kolicina = el.Kolicina,
                    Proizvod = new OutProizvodDTO
                    {
                        Id = el.Proizvod.Id,
                        Naziv = el.Proizvod.Naziv,
                        Cena = el.Proizvod.Cena,
                        Opis = el.Proizvod.Opis,
                        NacinKoriscenja = el.Proizvod.NacinKoriscenja,
                        Prodavac = null
                    }
                });
            }

            return outProdavacNarudzbina;
        }

        public async Task<OutProdavacNarudzbinaDTO> Get(Guid id, HttpContext context)
        {
            var narudzbina = await _db.Narudzbine.Include(n => n.Kupac).Include(n => n.Prodavac)
                .Include(n => n.ListaElemenata).ThenInclude(k => k.Proizvod).FirstOrDefaultAsync(a => a.Id == id);

            if (narudzbina == null)
                return null;

            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Prodavac ne postoji u sistemu.");

            if (narudzbina.Prodavac.Id != user.Id && narudzbina.Kupac.Id != user.Id)
                throw new ErrorException(ErrorCode.OrderAccessError, "Nemate pravo da pristupite ovoj narudzbini.");

            var outProdavacNarudzbina = new OutProdavacNarudzbinaDTO
            {
                Id = narudzbina.Id,
                StatusNarudzbine = narudzbina.StatusNarudzbine,
                VremeIsporukeUDanima = narudzbina.VremeIsporukeUDanima,
                Prodavac = new Account
                {
                    Address = narudzbina.Prodavac.Address,
                    Email = narudzbina.Prodavac.Email,
                    FirstName = narudzbina.Prodavac.FirstName,
                    LastName = narudzbina.Prodavac.LastName,
                    PhoneNumber = narudzbina.Prodavac.PhoneNumber
                },
                ListaElemenata = new List<OutElementKorpeDTO>()
            };
            foreach (var el in narudzbina.ListaElemenata)
            {
                outProdavacNarudzbina.ListaElemenata.Add(new OutElementKorpeDTO
                {
                    Kolicina = el.Kolicina,
                    Proizvod = new OutProizvodDTO
                    {
                        Id = el.Proizvod.Id,
                        Naziv = el.Proizvod.Naziv,
                        Cena = el.Proizvod.Cena,
                        Opis = el.Proizvod.Opis,
                        NacinKoriscenja = el.Proizvod.NacinKoriscenja,
                        Prodavac = null
                    }
                });
            }

            return outProdavacNarudzbina;
        }
    }
}
