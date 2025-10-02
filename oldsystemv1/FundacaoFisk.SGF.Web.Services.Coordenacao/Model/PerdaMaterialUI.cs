using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class PerdaMaterialUI : TO
    {
        public int cd_perda_material { get; set; }

        public System.DateTime dt_perda_material { get; set; }

        public int cd_movimento { get; set; }
        public int? nm_movimento { get; set; }
        public string dc_serie_movimento { get; set; }

        public string no_aluno { get; set; }

        public int cd_contrato { get; set; }
        public int? nm_contrato { get; set; }
        
        public List<ItemMovimento> itensPerdaMaterial { get; set; }

        
        public byte id_status_perda { get; set; }

        public string items
        {
            get
            {
                if (itensPerdaMaterial != null && itensPerdaMaterial.Count > 0)
                {
                    return String.Join("|", itensPerdaMaterial.Select(x => x.dc_item_movimento).ToList());
                }
                else
                {
                    return null;
                }
             }
        }

        public string dc_status_perda
        {
            get
            {
                if (id_status_perda == 1)
                {
                    return "Aberto";
                }else if (id_status_perda == 2)
                {
                    return "Fechado";
                }

                return "";
            }
        }

        public string dc_nm_movimento
        {
            get
            {
                string numero = "";
                numero = nm_movimento.ToString() + dc_serie_movimento == null ? "" : nm_movimento.ToString() + "-" + dc_serie_movimento;
                return numero;
            }
        }

        public string dta_perda_material
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_perda_material);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_curso", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_contrato", "Contrato", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dc_nm_movimento", "Movimento", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("dta_perda_material", "Emissão", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("items", "Item", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dc_status_perda", "Status", AlinhamentoColuna.Center, "0.7000in"));

                return retorno;
            }
        }
    }
}