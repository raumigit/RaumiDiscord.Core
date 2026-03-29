using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    /// <summary>
    /// gameCodeModel を CRUD（取得・更新・作成・削除）するための API コンポーネントです。<br/>
    /// データベースへのアクセスは Entity Framework Core の DeltaRaumiDbContext で行います。
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class GameCodesController : ControllerBase
    {
        private readonly DeltaRaumiDbContext _context;
        /// <summary>
        /// GiftCodesControllerのコンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public class GiftCodesController : ControllerBase
        {
            private readonly IGameCodeService _service;
            /// <summary>
            /// Initializes a new instance of the GiftCodesController class with the specified game code service.
            /// </summary>
            /// <param name="service">The service used to manage and validate game gift codes. Cannot be null.</param>
            public GiftCodesController(IGameCodeService service) => _service = service;

            [HttpGet("public")]
            public async Task<IActionResult> GetPublic()
                => Ok(await _service.GetActivePublicCodesAsync());
        }

        // GET: api/gameCodeModels
        /// <summary>
        /// データベース内の全 gameCodeModel レコードを取得します。<br/>
        /// <para>戻り値は JSON 配列形式で、各要素が gameCodeModel のプロパティを含みます。</para>
        /// </summary>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameCodeModel>>> GetgameCodeModels()
        {
            return await _context.GameCodeModels.ToListAsync();
        }

        // GET: api/gameCodeModels/5
        /// <summary>
        /// 指定した ID を持つ gameCodeModel を取得します。<br/>
        /// <paramref name="id"/> に該当するレコードが存在しない場合は 404 NotFound が返ります。<br/>
        /// 成功時は対象オブジェクトを JSON で返却します。
        /// </summary>
        /// <param name="id">取得したい gameCodeModel の ID。</param>

        [HttpGet("{id}")]
        public async Task<ActionResult<GameCodeModel>> GetgameCodeModel(uint id)
        {
            var gameCodeModel = await _context.GameCodeModels.FindAsync(id);

            if (gameCodeModel == null)
            {
                return NotFound();
            }

            return gameCodeModel;
        }

        // PUT: api/gameCodeModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 指定した ID を持つ gameCodeModel を更新します。<br/>
        /// リクエストボディに渡されたオブジェクトの Id が URL パラメータと一致しない場合は 400 BadRequest、<br/>
        /// レコードが存在しない場合は 404 NotFound を返します。<br/>
        /// 成功時は 204 NoContent を返却します。
        /// </summary>
        /// <param name="id">更新対象の gameCodeModel の ID。</param>
        /// <param name="gameCodeModel">更新内容を含むオブジェクト。</param>

        [HttpPut("{id}")]
        //[Authhorize(Roles="Admin")]
        public async Task<IActionResult> PutGamecodeModel(uint id, GameCodeModel gameCodeModel)
        {
            if (id != gameCodeModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameCodeModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!gameCodeModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/gameCodeModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 新しい gameCodeModel をデータベースに追加します。<br/>
        /// 成功すると 201 Created が返り、Location ヘッダーに作成したリソースの URL が設定されます。
        /// </summary>
        /// <param name="gameCodeModel">作成対象となるオブジェクト。</param>

        [HttpPost]
        public async Task<ActionResult<GameCodeModel>> PostgameCodeModel(GameCodeModel gameCodeModel)
        {
            _context.GameCodeModels.Add(gameCodeModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetgameCodeModel", new { id = gameCodeModel.Id }, gameCodeModel);
        }

        // DELETE: api/gameCodeModels/5
        /// <summary>
        /// 指定した ID を持つ gameCodeModel を削除します。<br/>
        /// レコードが見つからない場合は 404 NotFound、成功時は 204 NoContent を返却します。
        /// </summary>
        /// <param name="id">削除対象の gameCodeModel の ID。</param>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletegameCodeModel(uint id)
        {
            var gameCodeModel = await _context.GameCodeModels.FindAsync(id);
            if (gameCodeModel == null)
            {
                return NotFound();
            }

            _context.GameCodeModels.Remove(gameCodeModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool gameCodeModelExists(uint id)
        {
            return _context.GameCodeModels.Any(e => e.Id == id);
        }
    }
}
