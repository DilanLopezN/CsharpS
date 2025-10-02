using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class ComponentesPessoaUI
    {
        public IEnumerable<TipoEnderecoSGF> tiposEndereco { get; set; }
        public IEnumerable<TipoLogradouroSGF> tiposLogradouro { get; set; }
        public IEnumerable<EstadoUI> estadosUI { get; set; }
        public IEnumerable<ClasseTelefoneSGF> classesTelefone { get; set; }
        public IEnumerable<TipoTelefoneSGF> tiposTelefone { get; set; }
        public IEnumerable<OrgaoExpedidor> orgaosExpedidores { get; set; }
        public IEnumerable<PapelSGF> papeis { get; set; }
        public IEnumerable<Operadora> operadoras { get; set; }
        public IEnumerable<QualifRelacionamento> qualifRelacionamentos { get; set; }
        public IEnumerable<GrupoEstoque> gruposEstoques { get; set; }
    }
}
