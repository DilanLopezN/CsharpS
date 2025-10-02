using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess
{
    public interface IEscolaDataAccess : IGenericRepository<Escola>
    {  
        IEnumerable<EscolaUI> getEscolaByDesc(SearchParameters paranetrs, string desc, bool inicio, bool? ativo, string cnpj, string fantasia, int cdUsuario);
        EscolaUI getEscolaForEdit(int cd_pessoa_empresa);
        IEnumerable<EscolaUI> findEscolasSecionadas(int cdItem, int cd_usuario, bool masterGeral);
        IEnumerable<PessoaSearchUI> getSearchEscolas(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaNotWithItem(SearchParameters parametros, string desc, string cnpj, string fantasia, int cd_item, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaNotWithKit(SearchParameters parametros, string desc, List<int> empresas, string cnpj, string fantasia, int cd_item, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaHasItem(int cd_item);
        IEnumerable<PessoaSearchUI> getEscolaHasTpDesc(int cdTpDesc);
        bool verificaHorarioOcupado(int cd_pessoa_escola, TimeSpan hr_ini, TimeSpan hr_fim);
        int? getCodigoFranquia(int cd_escola, int id_aplicacao);
        bool? verifcaEstadoEscAluno(int cd_pessoa_escola, int cd_aluno);
        IEnumerable<UsuarioWebSGF> getUsuariosById(List<int> cdUsers);
        void deleteUsuarioContext(UsuarioWebSGF user);
        IEnumerable<UsuarioUISearch> getUsuarioSearchFKFollowUp(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa,
                                                               int cd_usuario_logado,int tipoPesq, string usuariologado, Int32[] codEscolas);
        IEnumerable<PessoaSearchUI> getEscolasFollowUp(int cd_follow_up, int cd_escola);
        IEnumerable<PessoaSearchUI> getSearchEscolasFKFollowUp(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio, List<int> cdsEmpresa, bool masterGeral, int? cd_estado, int? cd_cidade);
        bool getEmpresaPropria(int cd_escola);
        DataTable getLoginEscola(DateTime dt_analise, bool id_login, byte id_matricula);
    }
}
