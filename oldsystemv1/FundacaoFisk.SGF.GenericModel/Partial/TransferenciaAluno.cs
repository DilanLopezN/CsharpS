using System;
using System.Collections.Generic;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TransferenciaAluno 
    {

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
        public HistoricoAlunoReport historicoAlunoReport { get; set; }


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
                retorno.Add(new DefinicaoRelatorio("no_unidade_destino", "Unidade de Destino", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("cpf", "CPF", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("email_origem_enviado", "Email Enviado", AlinhamentoColuna.Left, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("dta_status", "Data", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_motivo_transferencia_aluno", "Motivo Transferência", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_raf", "RAF", AlinhamentoColuna.Left, "0.8500in"));
                retorno.Add(new DefinicaoRelatorio("no_status", "Status", AlinhamentoColuna.Left, "0.8500in"));


                return retorno;
            }
        }


        public static TransferenciaAluno changeValueTransferenciaAluno(TransferenciaAluno transferenciaAlunoBd,
            TransferenciaAluno transferenciaAlunoView)
        {
            transferenciaAlunoBd.cd_escola_destino = transferenciaAlunoBd.cd_escola_destino != transferenciaAlunoView.cd_escola_destino ? transferenciaAlunoView.cd_escola_destino : transferenciaAlunoBd.cd_escola_destino;
            transferenciaAlunoBd.cd_aluno_origem = transferenciaAlunoBd.cd_aluno_origem != transferenciaAlunoView.cd_aluno_origem ? transferenciaAlunoView.cd_aluno_origem : transferenciaAlunoBd.cd_aluno_origem;
            transferenciaAlunoBd.cd_motivo_transferencia = transferenciaAlunoBd.cd_motivo_transferencia != transferenciaAlunoView.cd_motivo_transferencia ? transferenciaAlunoView.cd_motivo_transferencia : transferenciaAlunoBd.cd_motivo_transferencia;
            transferenciaAlunoBd.dc_email_origem = transferenciaAlunoBd.dc_email_origem != transferenciaAlunoView.dc_email_origem ? transferenciaAlunoView.dc_email_origem : transferenciaAlunoBd.dc_email_origem;
            transferenciaAlunoBd.dc_email_destino = transferenciaAlunoBd.dc_email_destino != transferenciaAlunoView.dc_email_destino ? transferenciaAlunoView.dc_email_destino : transferenciaAlunoBd.dc_email_destino;
            transferenciaAlunoBd.id_status_transferencia = transferenciaAlunoBd.id_status_transferencia != transferenciaAlunoView.id_status_transferencia ? transferenciaAlunoView.id_status_transferencia : transferenciaAlunoBd.id_status_transferencia;
            transferenciaAlunoBd.no_arquivo_historico = transferenciaAlunoBd.no_arquivo_historico != transferenciaAlunoView.no_arquivo_historico ? transferenciaAlunoView.no_arquivo_historico : transferenciaAlunoBd.no_arquivo_historico;
            transferenciaAlunoBd.pdf_historico = transferenciaAlunoBd.pdf_historico != transferenciaAlunoView.pdf_historico ? transferenciaAlunoView.pdf_historico : transferenciaAlunoBd.pdf_historico;
            return transferenciaAlunoBd;
        }

        public static TransferenciaAluno changeValueTransferenciaAlunoReceber(TransferenciaAluno transferenciaAlunoBd,
            TransferenciaAluno transferenciaAlunoView)
        {
            transferenciaAlunoBd.dc_email_origem = transferenciaAlunoBd.dc_email_origem != transferenciaAlunoView.dc_email_origem ? transferenciaAlunoView.dc_email_origem : transferenciaAlunoBd.dc_email_origem;
            transferenciaAlunoBd.dc_email_destino = transferenciaAlunoBd.dc_email_destino != transferenciaAlunoView.dc_email_destino ? transferenciaAlunoView.dc_email_destino : transferenciaAlunoBd.dc_email_destino;
            transferenciaAlunoBd.id_status_transferencia = transferenciaAlunoBd.id_status_transferencia != transferenciaAlunoView.id_status_transferencia ? transferenciaAlunoView.id_status_transferencia : transferenciaAlunoBd.id_status_transferencia;
            return transferenciaAlunoBd;
        }
        public class HistoricoAlunoReport
        {
            public string produtos { get; set; }
            public string turmaAvaliacao { get; set; }
            public string estagioAvaliacao { get; set; }
            public string statusTitulo { get; set; }
            public bool mostrarEstagio { get; set; }
            public bool mostrarAtividade { get; set; }
            public bool mostrarObservacao { get; set; }
            public bool mostrarFollow { get; set; }
            public bool mostrarItem { get; set; }
            public int cd_aluno { get; set; }
            public string no_aluno { get; set; }
        }

    }
}