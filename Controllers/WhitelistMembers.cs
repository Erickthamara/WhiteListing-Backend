using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteListing_Backend.Models;
using WhiteListing_Backend.SupabaseModels;
using StatusOption = WhiteListing_Backend.Models.StatusOption;

namespace WhiteListing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WhitelistMembers : ControllerBase
    {

        private HttpClient _httpClient;
        private readonly Supabase.Client _supabase;


        public WhitelistMembers(HttpClient httpClient, Supabase.Client Supabase)
        {
            _httpClient = httpClient;
            _supabase = Supabase;

        }

        [HttpGet("GetWhitelistMembers")]
        public async Task<Whitelisting_MemberDTO[]> GetWhitelistMembers()
        {
            var response = await _supabase.From<SupabaseWhitelistMembers>().Get();

            if (response?.Models == null)
                return Array.Empty<Whitelisting_MemberDTO>();

            var dtoList = response.Models.Select(member => new Whitelisting_MemberDTO
            {
                //Id = member.ID,
                Member_Name = member.Member_Name,
                Member_No = member.Member_No,
                Id_No = member.Id_No,
                Phone_No = member.Phone_No,
                Client_id = member.Client_id,
                Status = (Models.StatusOption)member.Status
            }).ToArray();

            return dtoList;


        }

        [HttpPost("PostNewWhitelistMembers")]
        public async Task<IActionResult> PostNewWhitelistMembers([FromBody] Whitelisting_MemberDTO[] membersApplied)
        {
            if (membersApplied is null)
            {
                throw new ArgumentNullException(nameof(membersApplied));
            }

            if (membersApplied.Length == 0)
            {
                return BadRequest("No members provided for whitelisting.");
            }
            var existingMembers = await CheckIfMemberAlreadyExists(membersApplied);
            if (existingMembers.Count > 0)
            {
                return BadRequest($"The following members already exist:: {existingMembers}");
            }

            foreach (Whitelisting_MemberDTO member in membersApplied)
            {
                var supabaseMember = new SupabaseWhitelistMembers
                {
                    ID = new Guid(),
                    Member_Name = member.Member_Name,
                    Member_No = member.Member_No,
                    Id_No = member.Id_No,
                    Phone_No = member.Phone_No,
                    Client_id = member.Client_id,
                    Status = (SupabaseModels.StatusOption)StatusOption.Pending,
                };


                var response = await _supabase.From<SupabaseWhitelistMembers>().Insert(supabaseMember);

                if (response?.Models == null)
                {
                    throw new Exception("Failed to insert member into the database.");
                }
            }

            return Ok(new { message = "Members successfully added to the whitelist." });

        }

        [HttpPost("PostNewWhitelistExcel")]
        public IActionResult PostNewWhitelistMembersExcelFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            //// Save file to disk
            //var savePath = Path.Combine("UploadedFiles", file.FileName);
            //using var fs = new FileStream(savePath, FileMode.Create);
            //await file.CopyToAsync(fs);

            return Ok(new { message = "File successfully added." });

        }

        protected async Task<List<Whitelisting_MemberDTO>> CheckIfMemberAlreadyExists(Whitelisting_MemberDTO[] members)
        {
            List<Whitelisting_MemberDTO> existingMembers = new List<Whitelisting_MemberDTO>();
            foreach (var member in members)
            {

                var existingMember = await _supabase.From<SupabaseWhitelistMembers>()
                    .Select(mem => new object[] { mem.Id_No, mem.Member_No, mem.Member_Name, mem.Phone_No, mem.Client_id, mem.Status })
                    .Where(mem => mem.Id_No == member.Id_No)
                    .Where(mem => mem.Member_No == member.Member_No)
                    .Where(mem => mem.Client_id == member.Client_id)
                    .Single();

                if (existingMember != null)
                {
                    existingMembers.Add(new Whitelisting_MemberDTO
                    {
                        Member_Name = existingMember.Member_Name,
                        Member_No = existingMember.Member_No,
                        Id_No = existingMember.Id_No,
                        Phone_No = existingMember.Phone_No,
                        Client_id = existingMember.Client_id,
                        Status = (Models.StatusOption)existingMember.Status
                    });
                }

            }

            return existingMembers;

        }
    }
}
