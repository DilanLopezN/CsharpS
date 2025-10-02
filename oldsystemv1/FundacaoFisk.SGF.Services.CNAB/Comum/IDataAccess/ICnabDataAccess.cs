using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess
{
    public interface ICnabDataAccess : IGenericRepository<Cnab>
    {
        IEnumerable<Cnab> searchCnab(SearchParameters parametros, int cd_carteira, int cd_usuario, byte tipo_cnab, int status, DateTime? dtInicial,
                                                 DateTime? dtFinal, bool emissao, bool vencimento, string nossoNumero, int? nro_contrato, int cd_empresa,
                                                bool icnab, bool iboleto, int cd_responsavel, int cd_aluno);
        Cnab getGerarRemessa(int cd_escola, int cd_cnab);
        IEnumerable<UsuarioWebSGF> getUsuariosCnab(int cd_empresa, bool adm, int cdUsuario);
        IEnumerable<CarteiraCnab> getCarteirasCnab(int cd_empresa);
        bool verificarGerouCnab(int cd_escola, Int32[] cd_cnab, Int32[] tipos_cnab, byte status_cnab, bool is_titulo);
        Cnab getCnabEditView(int cd_cnab, int cdEscola);
        Cnab getCnabFull(int cd_cnab, int cd_escola);
        Cnab getCNABFullComTitulosCNAB(int cd_cnab, int cd_escola);
        Cnab getCnabReturnGrade(int cd_cnab, int cd_empresa);
        IEnumerable<Cnab> getCnabs(int[] cdCnabs, int cd_empresa);
        bool isResponsavelCNAB(Int32[] cd_cnab);
        bool isResponsavelTitulosCNAB(Int32[] cd_titulos_cnab);
        int getQtdCnabGeradoDia(string banco, string codigoBeneficiario, int cd_escola);
    }
}
