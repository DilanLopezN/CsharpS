using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class FuncionarioUI
    {
        public FuncionarioSGF funcionario { get; set; }
        public Professor professor { get; set; }
        public PessoaFisicaUI pessoaFisicaUI { get; set; }
        public bool habilitacaoNula { get; set; }
        public string cd_produtos_funcionario { get; set; }
        public ICollection<HabilitacaoProfessor> habilitacaoProfessor { get; set; }
        public ICollection<Horario> horarioSearchUI { get; set; }

        public static Professor changesValueProfessorViewToContext(Professor professorContext, Professor professorView)
        {
            if (professorContext.dc_numero_chapa != professorView.dc_numero_chapa)
                professorContext.dc_numero_chapa = professorView.dc_numero_chapa;
            if (professorContext.id_contratado != professorView.id_contratado)
                professorContext.id_contratado = professorView.id_contratado;
            if (professorContext.id_forma_pagamento != professorView.id_forma_pagamento)
                professorContext.id_forma_pagamento = professorView.id_forma_pagamento;
            if (professorContext.vl_pagamento_externo != professorView.vl_pagamento_externo)
                professorContext.vl_pagamento_externo = professorView.vl_pagamento_externo;
            if (professorContext.vl_pagamento_interno != professorView.vl_pagamento_interno)
                professorContext.vl_pagamento_interno = professorView.vl_pagamento_interno;
            if (professorContext.pc_termino_estagio != professorView.pc_termino_estagio)
                professorContext.pc_termino_estagio = professorView.pc_termino_estagio;
            if (professorContext.vl_termino_estagio != professorView.vl_termino_estagio)
                professorContext.vl_termino_estagio = professorView.vl_termino_estagio;
            if (professorContext.vl_rematricula != professorView.vl_rematricula)
                professorContext.vl_rematricula = professorView.vl_rematricula;
            professorContext.id_coordenador = professorView.id_coordenador;
            return professorContext;
        }

        public static HabilitacaoProfessor changesValueHabilProfessor(HabilitacaoProfessor HabilitacaoProfessorView, HabilitacaoProfessor habilitacaoProfessorContext)
        {
            if (habilitacaoProfessorContext.cd_produto != HabilitacaoProfessorView.cd_produto)
                habilitacaoProfessorContext.cd_produto = HabilitacaoProfessorView.cd_produto;
            if (habilitacaoProfessorContext.cd_estagio != HabilitacaoProfessorView.cd_estagio)
                habilitacaoProfessorContext.cd_estagio = HabilitacaoProfessorView.cd_estagio;
            return habilitacaoProfessorContext;
        }
    }
}
