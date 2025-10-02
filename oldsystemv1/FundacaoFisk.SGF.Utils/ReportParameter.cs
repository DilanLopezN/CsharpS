using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//N° maior 104
namespace FundacaoFisk.SGF.Utils {
    public enum TipoRelatorioSGFEnum {
        // Enums da secretaria:
        EscolaridadeSearch = 1,
        MidiaSearch = 13,
        TipoContatoSearch = 14,
        MtvMatSearch = 15,
        MtvNMatSearch = 16,
        MtvBolsaSearch = 17,
        MtvCancelBolsaSearch = 18,
        rtpAtividadeExtra = 91,
        rtpHistoricoAluno = 92,
        Prospect = 41,
        MatriculaSearch = 46,
        DesistenciaSearch = 50,
        AcaoFollowUp = 66,
        FollowUp = 67,
        AnoEscolar = 87,
        rtpAulaReposicao = 97,
        rtpFaixaEtaria = 98,

        // Enums da coordenação:
        SalaSearch = 2,
        EventoSearch = 3,
        ProdutoSearch = 4,
        DuracaoSearch = 5,
        TipoAtividadeExtraSearch = 40,
        AtividadeExtraSearch = 6,
        AtividadeExtraCompletoSearch = 39,
        MotivoDesistenciaSearch = 7,
        MotivoFaltaSearch = 8,
        ModalidadeSearch = 9,
        RegimeSearch = 10,
        ConceitoSearch = 11,
        FeriadoSearch = 12,
        EstagioSearch = 27,
        AvaliacaoSearch = 33,
        AtividadeExtra = 39,
        AvaliacaoTurmaSearch = 44,
        Turma = 45,
        DiarioAulaSearch = 47,
        AulaPersonalizadaSearch = 68,
        AvaliacaoParticipacaoSearch = 83,
        ParticipacaoSearch = 84,
        CargaProfessorSearch = 85,
        NivelSearch = 88,
        CalendarioEvento = 89,
        CalendarioAcademico = 90,

        //Enums do Financeiro
        GrupoEstoqueSearch = 19,
        MovimentacaoFinanceiraSearch = 20,
        TipoDescontoSearch = 21,
        TipoFinanceiroSearch = 22,
        TipoLiquidacaoSearch = 23,
        GrupoContaSearch = 42,
        SubgrupoContaSearch = 43,
        BancoSearch = 48,
        CarteiraCnabSearch = 57,
        LocalMovtoSearch = 49,
        PoliticaComercialSearch = 51,
        ContaCorrente = 53,
        Recibo = 54,
        Movimento = 55,
        FechamentoEstoque = 56,
        CnabBoleto = 58,
        RetornoCNAB = 59,
        TitulosCNAB = 60,
        TipoNotaFiscalSearch = 62,
        AliquotaUFSearch = 63,
        DadosNFSearch = 64,

        // Enum da intituição de ensino:
        UsuarioSearch = 24,
        EscolaSearch = 26,
        REPORTUSUARIO = 68,
        RELATORIODEUSO = 104,

        // Enums do Grupo:
        GrupoSearch = 25,

        // Enum do Curso:
        CursoSearch = 28,

        // Enum da Pessoa
        PessoaSearch = 29,

        // Enum da PoliticaDesconto
        PoliticaDescontoSearch = 30,

        CriterioAvaliacaoSearch = 31,
        TipoAvaliacaoSearch = 32,
        ProgramacaoCursoSearch = 34,
        TabelaCursoSearch = 36,
        //Funcionario
        FuncionarioSearch = 35,

        //Item/Serviço
        ItemServicoSearch = 37,
        KitSearch = 93,

        //aluno
        AlunoSearch = 38,
        AlunoRel = 61,

        BibliotecaSearch = 52,
        ModeloProgramacaoCursoSearch = 65,
        

        //Enums Auxiliares Pessoas
        PaisSearch = 71,
        EstadoSearch = 72,
        CidadeSearch = 73,
        BairroSearch = 74,
        DistritoSearch = 75,
        OperadoraSearch = 76,
        TipoLogradouroSearch = 77,
        TipoEnderecoSearch = 78,
        ClasseTelefoneSearch = 79,
        TipoTelefoneSearch = 80,
        AtividadeSearch = 81,
        Logradouro = 82,
        ReajusteAnualSearch = 86,

        //Controle de Faltas
        ControleFaltasSearch = 94,
        rtpControleFaltas = 95,
        AulasReposicaoSearch = 96,
        rtpAlunosSemTituloGerado = 97,
        OrgaoFinanceiroSearch = 99,

        FollowUpRel = 100,
        MensagemAvaliacaoSearch = 101,
        EnviarTransferenciaAlunoSearch = 102,
        ReceberTransferenciaAlunoSearch = 103,
        PerdaMaterialSearch = 104,
        AlunosCartaQuitacao = 105,


    }

    [Serializable]
    public class ReportParameter : Hashtable
    {
        //RAIZ
        public const String RELATORIO_DINAMICO = "RelatorioDinamico";
        public const String RELATORIO_ATIVIDADE_EXTRA = "AtividadeExtra";
        public const String RELATORIO_GESTAO_ATIVIDADE_EXTRA = "ReportGestaoAtividadeExtra";
        public const String RELATORIO_AULAREPOSICAO = "AulaReposicao";
        public const String RELATORIO_AULA_REPOSICAO_VIEW = "AulaReposicaoView";
        public const string RELATORIO_USO_SGF = "RptLoginEscola";
        //FINANCEIROS
        public const String RELATORIO_TITULO_PESSOA = "Financeiro/PosicaoFinanceira/RptTitulosPessoa";
        public const String RELATORIO_TITULO_DATA = "Financeiro/PosicaoFinanceira/RptTitulosData";
        public const String RELATORIO_TITULO_CONTA = "Financeiro/PosicaoFinanceira/RptTitulosConta";
        public const String RELATORIO_TITULOS_CNAB = "Financeiro/CNAB/RptTitulosCnab";
        public const String RELATORIO_TITULOS_RETORNO_CNAB = "Financeiro/CNAB/RptRetornoCnab";
        public const String RELATORIO_ALUNOS_SEM_TITULO_GERADO = "Financeiro/AlunosSemTituloGerado/RptAlunosSemTituloGerado";
        public const String RELATORIO_BAIXA_PESSOA = "Financeiro/PosicaoFinanceira/RptBaixasPessoa";
        public const String RELATORIO_BAIXA_DATA = "Financeiro/PosicaoFinanceira/RptBaixasData";
        public const String RELATORIO_BAIXA_CONTA = "Financeiro/PosicaoFinanceira/RptBaixasConta";
        public const String RELATORIO_RECIBO = "Financeiro/Baixa/RptReciboBaixa";
        public const String RELATORIO_RECIBO_PAGAMENTO = "Financeiro/Baixa/RptReciboPagamento";
        public const String RELATORIO_RECIBO_PAGAMENTO_AGRUPADO = "Financeiro/Baixa/RptReciboPagamentoAgrupado";
        public const String RELATORIO_RECIBO_MOVIMENTO = "Secretaria/Movimento/RptReciboMovimento";
        public const String RELATORIO_ESPELHO_MOVIMENTO = "Secretaria/Movimento/RptEspelhoMovimento";
        public const String RELATORIO_COPIA_ESPELHO_MOVIMENTO = "Secretaria/Movimento/Movimento";
        public const String RELATORIO_CONTA_CORRENTE = "Financeiro/ContaCorrente/RptContaCorrente";
        public const String RELATORIO_SALDO_FINANCEIRO = "Financeiro/SaldoFinanceiro/RptSaldoFinanceiro";
        public const String RELATORIO_RECIBO_CONFIRMACAO = "Secretaria/Matricula/RptReciboConfirmacao";
        //public const String RELATORIO_RECIBO_CONFIRMACAO_MOVIMENTO = "Coordenacao/Movimento/RptReciboConfirmacao";
        public const String RELATORIO_BALANCETE_MENSAL = "Financeiro/BalanceteMensal/RptBalancete";
        public const String RELATORIO_ESTOQUE = "Financeiro/Estoque/RptKardex";
        public const String RELATORIO_ESTOQUE_ITEM = "Financeiro/Estoque/RptListaItem";
        public const String RELATORIO_ESTOQUE_BIBLIOTECA = "Financeiro/Estoque/RptBiblioteca";
        public const String RELATORIO_INVENTARIO = "Financeiro/Estoque/RptInventario";
        public const String RELATORIO_ALUNO_RESTRICAO = "Financeiro/Restricao/RptAlunoRestricao";
        public const String RELATORIO_CHEQUES = "Financeiro/RptCheques";
        public const String RELATORIO_PERCENTUAL_TERMINO_ESTAGIO = "Financeiro/Funcionario/RptPercentualTerminoEstagio";
        public const String RELATORIO_COMISSAO_SECRETARIAS = "Financeiro/Funcionario/RptComissaoSecretarias";
        public const String RELATORIO_FECHAMENTO_CAIXA_SINT = "Financeiro/SaldoFinanceiro/RptFechamentoCaixaSint";
        public const string RELATORIO_CONTROLE_VENDAS_MATERIAL = "Financeiro/Estoque/RptContVendasMaterial";
        public const string CARTA_QUITACAO = "Financeiro/CartaQuitacao/CartaQuitacao";
        //SECRETARIA
        public const String RELATORIO_LISTAGEM_ANIVERSARIANTES = "Secretaria/RptListagemAniversariantes";
        public const String RELATORIO_CARNE = "Secretaria/Matricula/Carne";
        public const String RELATORIO_CARNE_MOVTO = "Secretaria/Movimento/CarneMovto";
        public const String RELATORIO_PROSPECT_ATENDIDO = "Secretaria/Prospect/RptProspectAtendido";
        public const String RELATORIO_PROSPECT_ATENDIDO_MOTIVO_NAO_MATRICULA = "Secretaria/Prospect/RptProspectAtendidosMotivoNaoMatricula";
        public const String RELATORIO_LISTAGEM_ENDERECOS_MMK = "Secretaria/MailMarketing/RptListagemEnderecoMMK";
        public const String RELATORIO_MATRICULA_ANALITICO = "Secretaria/Matricula/MatriculaAnalitico";
        public const String RELATORIO_MATRICULA_MOTIVO = "Secretaria/Matricula/MatriculaPorMotivo";
        public const String RELATORIO_MATRICULA_OUTROS = "Secretaria/Matricula/MatriculaOutros";
        public const String RELATORIO_MEDIA_ALUNOS = "Secretaria/RptMediaAlunos";
        public const String RELATORIO_HISTORICO_ALUNO = "Secretaria/HistoricoAluno/HistoricoAluno";
        public const String RELATORIO_FAIXA_ETARIA = "Secretaria/FaixaEtaria/FaixaEtaria";
        public const String RELATORIO_FOLLOW_UP = "Secretaria/FollowUp/FollowUp";
        public const string ETIQUETA = "Secretaria/MailMarketing/Etiqueta";
        public const String RELATORIO_ALUNO_CLIENTE = "Secretaria/RptAlunoCliente";
        //COORDENAÇÂO
        public const String RELATORIO_DIARIO_AULA = "Coordenacao/DiarioAula/RptDiarioAula";
        public const String RELATORIO_CONTROLE_FALTAS = "Coordenacao/ControleFaltas/ControleFalta";
        public const String RELATORIO_CARTA_AVISO = "Coordenacao/ControleFaltas/CartaAviso";
        public const String RELATORIO_DIARIO_AULA_QUEBRA = "Coordenacao/DiarioAula/RptDiarioAulaQuebra";
        public const String RELATORIO_PROSPECT_ATENDIDO_X_MATRICULA = "Secretaria/Prospect/RptProspectAtendidosXMatricula";
        public const String RELATORIO_COMPARATIVO_PROSPECTS_ATENDIDOS_PERIODOS = "Secretaria/Prospect/RptComparativoProspectsAtendidosPeriodos";
        public const String RELATORIO_PAGAMENTO_PROFESSORES = "Coordenacao/RptPagamentoProfessores";
        public const String RELATORIO_CONTROLE_SALA = "Coordenacao/Sala/RptControleSala";
        public const String RELATORIO_EVENTOS = "Coordenacao/DiarioAula/RptEventos";
        public const String RELATORIO_EVENTOS_QUEBRA_TURMA = "Coordenacao/DiarioAula/RptEventosQuebraTurma";
        public const String RELATORIO_DIARIO_AULA_PROGRAMACAO = "Coordenacao/DiarioAula/RptDiarioAulaProgramacao";
        public const String RELATORIO_DIARIO_AULA_PROGRAMACAO_QUEBRA_TURMA = "Coordenacao/DiarioAula/RptDiarioAulaProgramacaoQuebraTurma";
        public const string RELATORIO_LISTAGEM_TURMAS = "Coordenacao/Turma/RptTurma";
        public const string RELATORIO_TURMAS_ENCERRAR = "Coordenacao/Turma/RptTurmasEncerrar";
        public const string RELATORIO_TURMAS_NOVAS = "Coordenacao/Turma/RptTurmasNovas";
        public const string RELATORIO_AVALIACAO = "Coordenacao/Turma/RptAvaliacao";
        public const string RELATORIO_PROGRAMACAO_AULAS_TURMA = "Coordenacao/Turma/RptProgramacaoAulasTurma";
        public const String RELATORIO_BOLSISTAS = "Coordenacao/RptBolsistas";
        public const String RELATORIO_AULA_PERSONALIZADA = "Coordenacao/RptAulaPersonalizada";
        public const String RELATORIO_TURMA_MATRICULA_MATERIAL = "Coordenacao/Turma/RptTurmaMatriculaMaterial";
    }
}

