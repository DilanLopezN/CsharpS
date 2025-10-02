using System;
using System.Collections.Generic;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class TransferenciaAlunoUI  : TO
    {
        public int cd_transferencia_aluno { get; set; }
        public int cd_escola_origem { get; set; }
        public int cd_escola_destino { get; set; }
        public int cd_aluno_origem { get; set; }
        public Nullable<int> cd_aluno_destino { get; set; }
        public int cd_motivo_transferencia { get; set; }
        public System.DateTime dt_cadastro_transferencia { get; set; }
        public Nullable<System.DateTime> dt_solicitacao_transferencia { get; set; }
        public Nullable<System.DateTime> dt_confirmacao_transferencia { get; set; }
        public Nullable<System.DateTime> dt_transferencia { get; set; }
        public string dc_email_origem { get; set; }
        public string dc_email_destino { get; set; }
        public byte id_status_transferencia { get; set; }
        public bool id_email_origem { get; set; }
        public bool id_email_destino { get; set; }
        public string no_arquivo_historico { get; set; }
        public string pdf_historico { get; set; }

        public enum StatusTransferenciaAluno
        {
            CADASTRADA = 0,
            SOLICITADA = 1,
            APROVADA = 2,
            EFETUADA = 3,
            RECUSADA = 9,

        }
        public string cpf { get; set; }
        public string no_aluno { get; set; }
        public string no_motivo_transferencia_aluno { get; set; }
        public string nm_raf { get; set; }
        public string no_unidade_destino { get; set; }
        public string no_unidade_origem { get; set; }


        public string email_origem_enviado
        {
            get
            {
                if (id_email_origem == true)
                    return "Sim";
                return "Não";
            }

        }
        public string email_destino_enviado
        {
            get
            {
                if (id_email_destino == true)
                    return "Sim";
                return "Não";
            }

        }

        public string dta_cadastro_transferencia
        {
            get
            {
                string retorno = String.Empty;
                if (dt_cadastro_transferencia != null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_cadastro_transferencia);

                return retorno;
            }
        }

        public string dta_solicitacao_transferencia
        {
            get
            {
                string retorno = String.Empty;
                if (dt_solicitacao_transferencia != null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_solicitacao_transferencia);

                return retorno;
            }
        }

        public string dta_confirmacao_transferencia
        {
            get
            {
                string retorno = String.Empty;
                if (dt_confirmacao_transferencia != null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_confirmacao_transferencia);

                return retorno;
            }
        }

        public string dta_transferencia
        {
            get
            {
                string retorno = String.Empty;
                if (dt_transferencia != null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_transferencia);

                return retorno;
            }
        }

        public string dta_status
        {
            get
            {
                string retorno = String.Empty;
                switch (id_status_transferencia)
                {
                    case (int)StatusTransferenciaAluno.CADASTRADA:
                        if (dt_cadastro_transferencia != null)
                            retorno = String.Format("{0:dd/MM/yyyy}", dt_cadastro_transferencia);//ToLocalTime());
                        break;
                    case (int)StatusTransferenciaAluno.SOLICITADA:
                        if (dt_solicitacao_transferencia != null)
                            retorno = String.Format("{0:dd/MM/yyyy}", dt_solicitacao_transferencia);//ToLocalTime());
                        break;
                    case (int)StatusTransferenciaAluno.APROVADA:
                    case (int)StatusTransferenciaAluno.RECUSADA:
                        if (dt_confirmacao_transferencia != null)
                            retorno = String.Format("{0:dd/MM/yyyy}", dt_confirmacao_transferencia);//ToLocalTime());
                        break;
                    case (int)StatusTransferenciaAluno.EFETUADA:
                        if (dt_transferencia != null)
                            retorno = String.Format("{0:dd/MM/yyyy}", dt_transferencia);//ToLocalTime());
                        break;
                }

                return retorno;
            }
        }

        public string no_status
        {
            get
            {
                string retorno = String.Empty;
                switch (id_status_transferencia)
                {
                    case (int)StatusTransferenciaAluno.CADASTRADA:
                        retorno = "Cadastrada";
                        break;
                    case (int)StatusTransferenciaAluno.SOLICITADA:
                        retorno = "Solicitada";
                        break;
                    case (int)StatusTransferenciaAluno.APROVADA:
                        retorno = "Aprovada";
                        break;
                    case (int)StatusTransferenciaAluno.RECUSADA:
                        retorno = "Recusada";
                        break;
                    case (int)StatusTransferenciaAluno.EFETUADA:
                        retorno = "Efetuada";
                        break;
                }

                return retorno;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_item", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_unidade_destino", "Unidade de Destino", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("cpf", "CPF", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("email_origem_enviado", "Email Enviado", AlinhamentoColuna.Left, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("dta_status", "Data", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_motivo_transferencia_aluno", "Motivo", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_raf", "Raf", AlinhamentoColuna.Left, "0.8500in"));
                retorno.Add(new DefinicaoRelatorio("no_status", "Status", AlinhamentoColuna.Left, "1.0000in"));


                return retorno;
            }
        }


        
    }
}