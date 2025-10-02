using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Escola {

        public string nome_temp_assinatura_certificado {get;set;}
        public string nome_assinatura_certificado_anterior { get; set; }
        
        public string dtaInicio {
            get {
                return String.Format("{0:dd/MM/yyyy}", dt_inicio);
            }
        }

        public string dtaAbertura {
            get {
                return String.Format("{0:dd/MM/yyyy}", dt_abertura);
            }
        }

        public bool isMasterGeral { get; set; }

        //TODO: Karolina porque existe este método e para que está sendo usado no tipo de desconto?
        public static Escola formEmpresaEscola(Escola empresa)
        {
            Escola escola = new Escola();
            escola.cd_atividade_principal = empresa.cd_atividade_principal;
            escola.cd_endereco_principal = empresa.cd_endereco_principal;
            escola.cd_papel_principal = empresa.cd_papel_principal;
            escola.cd_pessoa = empresa.cd_pessoa;
            escola.cd_telefone_principal = empresa.cd_telefone_principal;
            escola.cd_tipo_sociedade = empresa.cd_tipo_sociedade;
            escola.dc_nom_presidente = empresa.dc_nom_presidente;
            escola.dc_num_cgc = empresa.dc_num_cgc;
            escola.dc_num_cnpj_cnab = empresa.dc_num_cnpj_cnab;
            escola.dc_num_insc_estadual = empresa.dc_num_insc_estadual;
            escola.dc_num_insc_municipal = empresa.dc_num_insc_municipal;
            escola.dc_num_pessoa = empresa.dc_num_pessoa;
            escola.dc_reduzido_pessoa = empresa.dc_reduzido_pessoa;
            escola.dc_registro_junta_comercial = empresa.dc_registro_junta_comercial;
            escola.dt_baixa = empresa.dt_baixa;
            escola.dt_cadastramento = empresa.dt_cadastramento;
            escola.dt_registro_junta_comercial = empresa.dt_registro_junta_comercial;
            //    escola.dta_cadastro = empresa.dta_cadastro;
            escola.hr_cadastro = empresa.hr_cadastro;
            escola.hr_final = empresa.hr_final;
            escola.hr_inicial = empresa.hr_inicial;
            escola.id_escola_ativa = empresa.id_escola_ativa;
            escola.id_exportado = empresa.id_exportado;
            escola.id_pessoa_empresa = empresa.id_pessoa_empresa;
            escola.img_pessoa = empresa.img_pessoa;
            escola.nm_natureza_pessoa = empresa.nm_natureza_pessoa;
            escola.nm_empresa_integracao = empresa.nm_empresa_integracao;
            escola.nm_cliente_integracao = empresa.nm_cliente_integracao;
            escola.no_pessoa = empresa.no_pessoa;
            escola.txt_obs_pessoa = empresa.txt_obs_pessoa;

            return escola;

        }
    }
}
