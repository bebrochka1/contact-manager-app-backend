using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using Test.Abstractions;
using Test.Data.Models;
using Test.Data.Repositories;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private IValidator<Contact> _validator;

        public ContactsController(IContactRepository contactRepository, IValidator<Contact> validator)
        {
            _validator = validator;
            _contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            return Ok(await _contactRepository.GetAll());
        }

        [HttpGet("clear")]
        public async Task<IActionResult> RemoveContacts()
        {
            await _contactRepository.Clear();
            await _contactRepository.Save();
            return Ok("Contacts cleared");
        }

        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateContact(Guid Id, [FromBody] Contact contact)
        {
            if (contact != null) 
            {
                var validationResults = await _validator.ValidateAsync(contact);

                if (validationResults.IsValid) 
                {
                    await _contactRepository.Update(Id, contact);
                    await _contactRepository.Save();
                    return Ok();
                }
                else
                {
                    return BadRequest($"Contact to update is not valid {JsonSerializer.Serialize(validationResults.Errors.ToList())}");
                }
            }

            return BadRequest($"No data provided");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await _contactRepository.Clear();
                await _contactRepository.Save();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var csvData = await reader.ReadToEndAsync();
                    var lines = csvData.Split('\n');
                    if(lines.Length < 100000)
                    {
                        foreach (var line in lines)
                        {
                            Console.WriteLine(line);
                            var values = line.Split(',');

                            if (values.Length == 5)
                            {
                                var contact = new Contact
                                {
                                    Name = values[0].Trim('"'),
                                    DateOfBirth = DateTime.ParseExact(values[1].Trim('"'), "dd.MM.yyyy", CultureInfo.InvariantCulture),
                                    Married = bool.Parse(values[2].Trim('"')),
                                    Phone = values[3].Trim('"'),
                                    Salary = decimal.Parse(values[4].Trim('"'))
                                };
                                Console.WriteLine("New contact added: " + contact.ToString());

                                var validationResult = await _validator.ValidateAsync(contact);

                                if (validationResult.IsValid)
                                {
                                    await _contactRepository.Add(contact);
                                }
                                else
                                {
                                    Console.WriteLine($"Error request invalid contact {contact.ToString()}");
                                    Console.WriteLine($"Error request invalid data");
                                    foreach(var error in validationResult.Errors)
                                    {
                                        Console.WriteLine(error.ToString());
                                    }
                                    return BadRequest($"Contact with phone number {contact.Phone} is not valid: {validationResult.Errors.ToList()}");
                                }
                            }
                        }
                        await _contactRepository.Save();
                        return Ok("File uploaded successfully");
                    }
                    else
                    {
                        Console.WriteLine("File is in incorrect format");
                        return BadRequest("File should contain be less than 100000 contacts and it's can't be empty");
                    }
                }
                
            }
            else
            {
                return BadRequest("No file uploaded");
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _contactRepository.Delete(Id);
            await _contactRepository.Save();
            return Ok();
        }
    }
}
