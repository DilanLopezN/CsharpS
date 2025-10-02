using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class TituloUI : TO
    {
        public int cd_pessoa_empresa {get;set;}
        public int cd_pessoa {get;set;}
        public int cd_pessoa_cliente { get; set; }
        public bool responsavel {get;set;}
        public int locMov {get;set;}
        public int natureza {get;set;}
        public int status {get;set;}
        public int? numeroTitulo {get;set;}
        public int? parcelaTitulo {get;set;}        
        public DateTime dt_venc_cnab { get; set; }
        public List<LocalMovto> locais { get; set; }
        public bool todosLocais { get; set; }
        public List<LocalMovto> locaisEscolhidos { get; set; }
        public List<TituloCnab> titulosGrade { get; set; }
        public List<TituloRetornoCNAB> titulosGradeRet { get; set; }
        public bool emissao {get;set;}
        public bool vencimento {get;set;}
        public byte id_tipo_cnab { get; set; }

        public byte id_tipo_retorno { get; set; }
        //Campos pesquisa view titulo cnab
        public int nro_contrato { get; set; }
        public byte id_cnab_tipo { get; set; }
        public int cd_produto { get; set; }
        public int cd_turma { get; set; }
        public int cd_aluno { get; set; }

        public int cd_local_movto_cartao { get; set; }
        public int cd_local_movto_banco { get; set; }
        public int? cd_local_movto { get; set; }
        public DateTime? dtInicial { get; set; }
        public DateTime? dtFinal { get; set; }
        public int status_tit_baixa_aut { get; set; }
        public int[] cdLocais
        {
            get
            {
                int[] cdLocais = null;
                int i;
                // Pegando códigos da Turma
                if (locais != null && locais.Count() > 0)
                {
                    i = 0;
                    int[] cdlocaisCont = new int[locais.Count()];
                    foreach (var c in locais)
                    {
                        cdlocaisCont[i] = c.cd_local_movto;
                        i++;
                    }
                    cdLocais = cdlocaisCont;
                }
                return cdLocais;
            }
        }

        public int[] cdLocaisEscolhidos
        {
            get
            {
                int[] cdLocaisEscolhidos = null;
                int i;
                // Pegando códigos da Turma
                if (locaisEscolhidos != null && locaisEscolhidos.Count() > 0)
                {
                    i = 0;
                    int[] cdlocaisCont = new int[locaisEscolhidos.Count()];
                    foreach (var c in locaisEscolhidos)
                    {
                        cdlocaisCont[i] = c.cd_local_movto;
                        i++;
                    }
                    cdLocaisEscolhidos = cdlocaisCont;
                }
                return cdLocaisEscolhidos;
            }
        }

        public int[] cdTituloExcludeSelect
        {
            get
            {
                int[] cdTitulos = null;
                int i;
                // Pegando códigos da Turma
                if (titulosGrade != null && titulosGrade.Count() > 0)
                {
                    i = 0;
                    int[] cdTitulosCont = new int[titulosGrade.Count()];
                    foreach (var c in titulosGrade)
                    {
                        cdTitulosCont[i] = c.cd_titulo;
                        i++;
                    }
                    cdTitulos = cdTitulosCont;
                }
                return cdTitulos;
            }
        }
    }
}
