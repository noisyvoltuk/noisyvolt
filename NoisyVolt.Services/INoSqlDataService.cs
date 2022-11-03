using NoisyVolt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoisyVolt.Services
{
    public interface INoSqlDataService
    {
        Task<string> SaveEmbedCard(EmbedCard embedCard);

        Task<List<EmbedCard>> GetEmbeds();
    }
}
