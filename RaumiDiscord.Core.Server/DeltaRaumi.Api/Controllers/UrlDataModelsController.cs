using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    /// <summary>
    /// UrlDataModel を CRUD（取得・更新・作成・削除）するための API コンポーネントです。<br/>
    /// データベースへのアクセスは Entity Framework Core の DeltaRaumiDbContext で行います。
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class UrlDataModelsController : ControllerBase
    {
        private readonly DeltaRaumiDbContext _context;
        /// <summary>
        /// UrlDataModelsControllerのコンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public UrlDataModelsController(DeltaRaumiDbContext context)
        {
            _context = context;
        }

        // GET: api/UrlDataModels
        /// <summary>
        /// データベース内の全 UrlDataModel レコードを取得します。<br/>
        /// <para>戻り値は JSON 配列形式で、各要素が UrlDataModel のプロパティを含みます。</para>
        /// </summary>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlDataModel>>> GetUrlDataModels()
        {
            return await _context.UrlDataModels.ToListAsync();
        }

        // GET: api/UrlDataModels/5
        /// <summary>
        /// 指定した ID を持つ UrlDataModel を取得します。<br/>
        /// <paramref name="id"/> に該当するレコードが存在しない場合は 404 NotFound が返ります。<br/>
        /// 成功時は対象オブジェクトを JSON で返却します。
        /// </summary>
        /// <param name="id">取得したい UrlDataModel の ID。</param>

        [HttpGet("{id}")]
        public async Task<ActionResult<UrlDataModel>> GetUrlDataModel(uint id)
        {
            var urlDataModel = await _context.UrlDataModels.FindAsync(id);

            if (urlDataModel == null)
            {
                return NotFound();
            }

            return urlDataModel;
        }

        // PUT: api/UrlDataModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 指定した ID を持つ UrlDataModel を更新します。<br/>
        /// リクエストボディに渡されたオブジェクトの Id が URL パラメータと一致しない場合は 400 BadRequest、<br/>
        /// レコードが存在しない場合は 404 NotFound を返します。<br/>
        /// 成功時は 204 NoContent を返却します。
        /// </summary>
        /// <param name="id">更新対象の UrlDataModel の ID。</param>
        /// <param name="urlDataModel">更新内容を含むオブジェクト。</param>

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUrlDataModel(uint id, UrlDataModel urlDataModel)
        {
            if (id != urlDataModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(urlDataModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrlDataModelExists(id))
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

        // POST: api/UrlDataModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 新しい UrlDataModel をデータベースに追加します。<br/>
        /// 成功すると 201 Created が返り、Location ヘッダーに作成したリソースの URL が設定されます。
        /// </summary>
        /// <param name="urlDataModel">作成対象となるオブジェクト。</param>

        [HttpPost]
        public async Task<ActionResult<UrlDataModel>> PostUrlDataModel(UrlDataModel urlDataModel)
        {
            _context.UrlDataModels.Add(urlDataModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUrlDataModel", new { id = urlDataModel.Id }, urlDataModel);
        }

        // DELETE: api/UrlDataModels/5
        /// <summary>
        /// 指定した ID を持つ UrlDataModel を削除します。<br/>
        /// レコードが見つからない場合は 404 NotFound、成功時は 204 NoContent を返却します。
        /// </summary>
        /// <param name="id">削除対象の UrlDataModel の ID。</param>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrlDataModel(uint id)
        {
            var urlDataModel = await _context.UrlDataModels.FindAsync(id);
            if (urlDataModel == null)
            {
                return NotFound();
            }

            _context.UrlDataModels.Remove(urlDataModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UrlDataModelExists(uint id)
        {
            return _context.UrlDataModels.Any(e => e.Id == id);
        }
    }
}
