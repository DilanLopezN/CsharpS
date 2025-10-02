using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class EnderecoUI
    {
       public int cd_endereco {get;set;}
       public int cd_loc_cidade {get;set;}
       public int cd_loc_estado {get;set;}
       public int cd_loc_pais {get;set;}
       public int cd_tipo_endereco {get;set;}
       public Nullable<int> cd_tipo_logradouro { get; set; }
       public Nullable<int> cd_loc_bairro { get; set; }
       public Nullable<int> cd_loc_logradouro { get; set; }
       public string dc_compl_endereco { get; set; }
       public string dc_num_cep { get; set; }
       public string dc_num_endereco { get; set; }
       public string rua { get; set; }
       public string bairro { get; set; }
       public string cidade { get; set; }
       public string noLocCidade { get; set; }
       public string noLocBairro {get; set;}
       public string noLocRua {get; set;}
       public string num_cep {get; set;}

       public static IEnumerable<EnderecoUI> fromEnderecoforEnderecoUI(ICollection<EnderecoSGF> enderecos,int? cdEndereco)
       {
           List<EnderecoUI> enderecoUIs = new List<EnderecoUI>();
           foreach (var endereco in enderecos)
           {
               if (endereco.cd_endereco != cdEndereco)
               {
                   enderecoUIs.Add(new EnderecoUI
                   {
                       cd_endereco = endereco.cd_endereco,
                       cd_loc_cidade = endereco.cd_loc_cidade,
                       cd_loc_estado = endereco.cd_loc_estado,
                       cd_loc_pais = endereco.cd_loc_pais,
                       cd_tipo_endereco = endereco.cd_tipo_endereco,
                       cd_tipo_logradouro = endereco.cd_tipo_logradouro,
                       cd_loc_bairro = endereco.cd_loc_bairro,
                       cd_loc_logradouro = endereco.cd_loc_logradouro,
                       dc_compl_endereco = endereco.dc_compl_endereco,
                       dc_num_endereco = endereco.dc_num_endereco,
                       rua = endereco.Logradouro.no_localidade,
                       bairro = endereco.Bairro.no_localidade,
                       cidade = endereco.Cidade.no_localidade
                   });
               }

           }
           //retorno.ativo = usuarioWeb.usuario_ativo;
           return enderecoUIs;
       }

       public static IEnumerable<EnderecoSGF> fromEnderecoUIForEndereco(IEnumerable<EnderecoUI> enderecosUI)
       {
           List<EnderecoSGF> enderecos = new List<EnderecoSGF>();
           foreach (var enderecoUI in enderecosUI)
           {
               enderecos.Add(new EnderecoSGF
                   {
                       cd_endereco = enderecoUI.cd_endereco,
                       cd_loc_cidade = enderecoUI.cd_loc_cidade,
                       cd_loc_estado = enderecoUI.cd_loc_estado,
                       cd_loc_pais = enderecoUI.cd_loc_pais,
                       cd_tipo_endereco = enderecoUI.cd_tipo_endereco,
                       cd_tipo_logradouro = enderecoUI.cd_tipo_logradouro,
                       cd_loc_bairro = enderecoUI.cd_loc_bairro,
                       cd_loc_logradouro = enderecoUI.cd_loc_logradouro,
                       dc_compl_endereco = enderecoUI.dc_compl_endereco,
                       num_cep = enderecoUI.dc_num_cep,
                       dc_num_endereco = enderecoUI.dc_num_endereco
                   });

           }
           //retorno.ativo = usuarioWeb.usuario_ativo;
           return enderecos;
       }
    }
}
