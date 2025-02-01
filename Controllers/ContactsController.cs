using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TestTask.Models;
using static TestTask.Models.DatabaseContext;

namespace TestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        //Инициализация базы данных
        DatabaseContext db;
        public ContactsController(DatabaseContext context)
        {
            db = context;
            if (!db.Contacts.Any())
            {
                db.Contacts.Add(new Contact { Name = "Vasiliy", Surname="Lanovoy", Patronymic="Semyonovich",Phone="+7 (956) 845-79-42" });
                db.Contacts.Add(new Contact { Name = "Victor", Surname= "Tsoi", Patronymic= "Tsoi", Phone = "+7 (942) 452-84-41" });
                db.Contacts.Add(new Contact { Name = "Likaoh", Surname="Horny", Patronymic="Lovkovich", Phone = "+7 (923) 648-64-08" });
                db.SaveChanges();
            }
        }

        //Запрос на получение всех записей из бд - /api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> Get()
        {
            return await db.Contacts.ToListAsync();
        }

        //Запрос на получение записи из бд по id - /api/Contacts/id/{id}
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Contact>> Get(int id)
        {
            Contact contact = await db.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (contact == null)
                return NotFound();
            return new ObjectResult(contact);
        }


        //Запрос на получение записей из бд по фамилии - /api/Contacts/surname/{Surname}
        //P.S. почему-то swagger не выдает значения. В Postmane работает
        [HttpGet("surname/{Surname}")]
        public async Task<ActionResult<Contact>> GetSurname(string surname)
        {
            var contacts = db.Contacts.AsQueryable();
            contacts = contacts.Where(c => c.Surname == surname);

            if (contacts == null)
                return NotFound();

            return Ok(await contacts.ToListAsync());
        }

        // Новая запись в бд 
        [HttpPost("post")]
        public async Task<ActionResult<Contact>> Post(Contact contact)
        {
            if (contact == null)
            {
                return BadRequest();
            }
            
            //Проверка корректности номера
            if (contact.Phone[0] != '+' || contact.Phone.Length < 12)
            {
                return BadRequest("Number is not correct. It must start with '+' and contain more than 11 numbers.");
            }
            
            //Изменение номера под стандартный вид
            contact.Phone = contact.Phone.Substring(0, contact.Phone.Length - 10) +
                            " (" + contact.Phone.Substring(contact.Phone.Length - 10, 3) +
                            ") " + contact.Phone.Substring(contact.Phone.Length - 7, 3) +
                            "-" + contact.Phone.Substring(contact.Phone.Length - 4, 2) +
                            "-" + contact.Phone.Substring(contact.Phone.Length - 2, 2);
            
            db.Contacts.Add(contact);
            await db.SaveChangesAsync();
            return Ok(contact);
        }

        // Удаление записи по id
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Contact>> Delete(int id)
        {
            Contact contact = db.Contacts.FirstOrDefault(x => x.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();
            return Ok(contact);
        }

    }
}
