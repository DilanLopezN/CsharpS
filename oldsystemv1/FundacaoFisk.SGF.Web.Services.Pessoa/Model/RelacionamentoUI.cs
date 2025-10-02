using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class RelacionamentoUI
    {
        public string cpfCnpj { get; set; }
        public string no_pessoa { get; set; }
        public string no_papel { get; set; }
        public int id_natureza_papel { get; set; }
        public bool ehSelecionado { get; set; }
        public bool ehSelectBd { get; set; }
        public bool ehRegBdEdit { get; set; }
        public bool ehRelacInverso { get; set; }
        public Nullable<int> cd_qualif_relacionamento { get; set; }
        public string desc_qualif_relacionamento { get; set; }
        public string dc_fone_mail { get; set; }

        public RelacionamentoSGF relacionamento { get; set; }
        public EnderecoSGF enderecoRelac { get; set; }
        public PessoaFisicaSGF pessoaFisicaRelac { get; set; }
        public PessoaJuridicaSGF pessoaJuridicaRelac { get; set; }
        public byte nm_natureza_pessoa { get; set; }

        //public string desc_parentesco
        //{
        //    get
        //    {
        //        string retorno = "";
        //        if (id_grau_parentesco != null)
        //            switch (id_grau_parentesco)
        //            {
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.MAE:
        //                    retorno = "Mãe";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.PAI:
        //                    retorno = "Pai";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.MADASTRA:
        //                    retorno = "Madastra";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.PADASTRO:
        //                    retorno = "Padastro";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.AVO:
        //                    retorno = "Avô(ó)";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.IRMA_O:
        //                    retorno = "Irmão(ã)";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.TIO_A:
        //                    retorno = "Tio(a)";
        //                    break;
        //                case (int)RelacionamentoSGF.TipoGrauParentesco.PRIMO_A:
        //                    retorno = "Primo(a)";
        //                    break;
        //            }
        //        return retorno;
        //    }
        //}

        public static RelacionamentoSGF changeValuesRelacionamento(RelacionamentoSGF relacContext, RelacionamentoSGF relacView)
        {
            if (relacContext.cd_pessoa_pai != relacView.cd_pessoa_pai)
                relacContext.cd_pessoa_pai = relacView.cd_pessoa_pai;
            if (relacContext.cd_pessoa_filho != relacView.cd_pessoa_filho)
                relacContext.cd_pessoa_filho = relacView.cd_pessoa_filho;
            if (relacContext.cd_papel_pai != relacView.cd_papel_pai)
                relacContext.cd_papel_pai = relacView.cd_papel_pai;
            if (relacContext.cd_papel_filho != relacView.cd_papel_filho)
                relacContext.cd_papel_filho = relacView.cd_papel_filho;
            return relacContext;
        }

        public static List<RelacionamentoSGF> toRelacionamentos(ICollection<RelacionamentoUI> relacionamentosUI)
        {
            List<RelacionamentoSGF> relacionamentos = new List<RelacionamentoSGF>();

            if (relacionamentosUI != null && relacionamentosUI.Count() > 0)

                foreach (var item in relacionamentosUI)
                {
                    RelacionamentoSGF relac = item.relacionamento;
                    relac.ehRelacInverso = item.ehRelacInverso;
                    if (item.nm_natureza_pessoa == 1)
                    {
                        if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                        {
                            item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                            relac.PessoaFilho = item.pessoaFisicaRelac;
                            relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            if (!string.IsNullOrEmpty(item.dc_fone_mail))
                                relac.PessoaFilho.Telefone = new TelefoneSGF { dc_fone_mail = item.dc_fone_mail };
                        }
                        relacionamentos.Add(relac);
                    }
                    else
                    {
                        if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                        {
                            item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                            item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                            relac.PessoaFilho = item.pessoaJuridicaRelac;
                            if (!string.IsNullOrEmpty(item.dc_fone_mail))
                                relac.PessoaFilho.Telefone = new TelefoneSGF { dc_fone_mail = item.dc_fone_mail };
                        }
                        relacionamentos.Add(relac);
                    }

                }
            return relacionamentos;
        }

      public static IEnumerable<RelacionamentoUI> fromRelacionamentoforRelacionamentoUI(ICollection<RelacionamentoSGF> relacionamentos, int cd_pessoa)
      {
          List<RelacionamentoUI> relacionamentoUIs = new List<RelacionamentoUI>();
          foreach (var relacionamento in relacionamentos)
          {
              string no_papel = "";
              string no_pessoa = "";
              int id_natureza_papel = 0;
              int? nm_tipo_papel = null;
              bool ehRelacInverso = false;
              string dc_fone_mail = "";
              //// CPF ou CNPJ:
              //if (relacionamento.PessoaFilho != null && relacionamento.PessoaFilho.cd_pessoa != cd_pessoa)
              //    cpfCnpj = relacionamento.PessoaFilho.nm_cpf_cgc;
              //else
              //    cpfCnpj = relacionamento.PessoaPai.nm_cpf_cgc;
              // Papel:
              if (relacionamento.RelacionamentoFilhoPapel != null && relacionamento.PessoaFilho.cd_pessoa != cd_pessoa)
              {
                  no_papel = relacionamento.RelacionamentoFilhoPapel.no_papel;
                  ehRelacInverso = false;
              }
              else
              {
                  no_papel = relacionamento.RelacionamentoPaiPapel.no_papel;
                  ehRelacInverso = true;
              }

              // Pessoa:
              if (relacionamento.RelacionamentoFilhoPapel != null && relacionamento.PessoaFilho.cd_pessoa != cd_pessoa){
                  if (relacionamento.PessoaFilho.Telefone != null)
                      dc_fone_mail = relacionamento.PessoaFilho.Telefone.dc_fone_mail;
                  no_pessoa = relacionamento.PessoaFilho.no_pessoa;
              }else{
                  if (relacionamento.PessoaPai.Telefone != null)
                      dc_fone_mail = relacionamento.PessoaPai.Telefone.dc_fone_mail;
                  no_pessoa = relacionamento.PessoaPai.no_pessoa;
              }

              // Natureza Papel:
              if(relacionamento.PessoaFilho.nm_natureza_pessoa != null && relacionamento.PessoaFilho.cd_pessoa != cd_pessoa)
                  id_natureza_papel = int.Parse(relacionamento.PessoaFilho.nm_natureza_pessoa + "");
              else
                  if(relacionamento.PessoaPai.nm_natureza_pessoa != null)
                      id_natureza_papel = int.Parse(relacionamento.PessoaPai.nm_natureza_pessoa + "");

              if (relacionamento.RelacionamentoFilhoPapel != null && relacionamento.PessoaFilho.cd_pessoa != cd_pessoa)
                  nm_tipo_papel = relacionamento.RelacionamentoFilhoPapel.nm_tipo_papel;
              else
                  nm_tipo_papel = relacionamento.RelacionamentoPaiPapel.nm_tipo_papel;

              relacionamentoUIs.Add(new RelacionamentoUI {
                  /*cd_relacionamento = relacionamento.cd_relacionamento,
                  cd_pessoa_pai = relacionamento.cd_pessoa_pai,
                  cd_pessoa_filho = relacionamento.cd_pessoa_filho,
                  cd_papel_filho = relacionamento.cd_papel_filho,
                  cd_papel_pai = relacionamento.cd_papel_pai,*/
                  relacionamento = relacionamento,
                  no_papel = no_papel,
                  no_pessoa = no_pessoa,
                  id_natureza_papel = id_natureza_papel,
                  //cpfCnpj = cpfCnpj,
                  //nm_tipo_papel = nm_tipo_papel,
                  ehSelecionado =  false,
                  ehSelectBd = true,
                  ehRegBdEdit = false,
                  ehRelacInverso = ehRelacInverso,
                  cd_qualif_relacionamento = relacionamento.cd_qualif_relacionamento,
                  desc_qualif_relacionamento = relacionamento.desc_qualif_relacionamento,
                  dc_fone_mail = dc_fone_mail
              });
          }
          return relacionamentoUIs;
      }

    }
}
