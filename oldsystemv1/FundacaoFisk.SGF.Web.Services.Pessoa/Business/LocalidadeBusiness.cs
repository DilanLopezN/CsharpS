using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using Componentes.Utils;
using System.Text;
using System.Transactions;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using Componentes.GenericBusiness.Excepion;
using System.Data.Entity;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Business
{
	public class LocalidadeBusiness : ILocalidadeBusiness
	{
		public ILocalidadeDataAccess DataAccessLoc { get; set; }
		public IPaisDataAccess DataAccessPais { get; set; }
		public IEstadoDataAccess DataAccessEstado { get; set; }
		public ITipoEnderecoDataAccess DataAccessTpEnd { get; set; }
		public IClasseTelefoneDataAccess DataAccessClasseTelefone { get; set; }
		public ITipoLogradouroDataAccess DataAccessTipoLogradouro { get; set; }
		public ITipoTelefoneDataAccess DataAccessTipoTelefone { get; set; }
		public IOperadoraDataAccess DataAccessOperadora { get; set; }
        public IAtividadeDataAccess DataAccessAtividade { get; set; }
		public IEnderecoDataAccess DataAccessEnd { get; set; }

		const byte RUA = 6, BAIRRO = 4, CIDADE = 3;

		public LocalidadeBusiness(ILocalidadeDataAccess dataAccessLoc, IPaisDataAccess dataAccessPais, IEstadoDataAccess dataAccessEstado,
								  ITipoEnderecoDataAccess dataAccessTpEnd, IClasseTelefoneDataAccess dataAccessClasseTelefone, ITipoLogradouroDataAccess dataAccessTipoLogradouro,
                                  ITipoTelefoneDataAccess dataAccessTipoTelefone, IOperadoraDataAccess dataAccessOperadora, IAtividadeDataAccess dataAccessAtividade, IEnderecoDataAccess dataAccessEnd)
		{
			if ((dataAccessLoc == null) || (dataAccessPais == null) || (dataAccessEstado == null) || 
				(dataAccessTpEnd == null) || (dataAccessClasseTelefone == null) || (dataAccessTipoLogradouro == null) ||
                (dataAccessTipoTelefone == null) || (dataAccessOperadora == null) || (dataAccessAtividade == null) || dataAccessEnd == null)
			{
				throw new ArgumentNullException("repository");
			}
			DataAccessLoc = dataAccessLoc;
			DataAccessPais = dataAccessPais;
			DataAccessEstado = dataAccessEstado;
			DataAccessTpEnd = dataAccessTpEnd;
			DataAccessClasseTelefone = dataAccessClasseTelefone;
			DataAccessTipoLogradouro = dataAccessTipoLogradouro;
			DataAccessTipoTelefone = dataAccessTipoTelefone;
			DataAccessOperadora = dataAccessOperadora;
            DataAccessAtividade = dataAccessAtividade;
			DataAccessEnd = dataAccessEnd;
		}

        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessLoc.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessPais.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessEstado.DB()).IdUsuario =
             ((SGFWebContext)this.DataAccessTpEnd.DB()).IdUsuario = ((SGFWebContext) this.DataAccessClasseTelefone.DB()).IdUsuario = ((SGFWebContext)  
              this.DataAccessTipoLogradouro.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTipoTelefone.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessOperadora.DB()).IdUsuario = 
             ((SGFWebContext)this.DataAccessEnd.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.DataAccessLoc.DB()).cd_empresa = ((SGFWebContext)this.DataAccessPais.DB()).cd_empresa =
                ((SGFWebContext)this.DataAccessEstado.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTpEnd.DB()).cd_empresa = ((SGFWebContext)
                this.DataAccessClasseTelefone.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTipoLogradouro.DB()).cd_empresa =
             ((SGFWebContext)this.DataAccessTipoTelefone.DB()).cd_empresa = ((SGFWebContext)this.DataAccessOperadora.DB()).cd_empresa = ((SGFWebContext)
             this.DataAccessEnd.DB()).cd_empresa = cd_empresa;
        }

        public void sincronizaContexto(DbContext db)
        {
            //DataAccessLoc.sincronizaContexto(db);
            //DataAccessPais.sincronizaContexto(db);
            //DataAccessEstado.sincronizaContexto(db);
            //DataAccessTpEnd.sincronizaContexto(db);
            //DataAccessClasseTelefone.sincronizaContexto(db);
            //DataAccessTipoLogradouro.sincronizaContexto(db);
            //DataAccessTipoTelefone.sincronizaContexto(db);
            //DataAccessOperadora.sincronizaContexto(db);
            //DataAccessAtividade.sincronizaContexto(db);
            //DataAccessEnd.sincronizaContexto(db);
        }

        #region Endereço
        public IEnumerable<LocalidadeSGF> GetAllEndereco()
        {
            return DataAccessLoc.GetAllEndereco();
        }

        public EnderecoSGF FindById(int codEndereco)
        {
            return DataAccessEnd.findById(codEndereco, false);
        }

        public EnderecoSGF getEnderecoByLogradouro(int cd_logradouro, string nm_cep)
        {
            if ((cd_logradouro == 0) && string.IsNullOrEmpty(nm_cep))
                throw new PessoaBusinessException(string.Format(Componentes.Utils.Messages.Messages.msgErroNaoLLogradouroValido), null,
                        PessoaBusinessException.TipoErro.ERRO_CONVERSAO_ENDERECO, false);
            EnderecoSGF endereco = DataAccessEnd.getEnderecoByLogradouro(cd_logradouro, nm_cep);
            if (endereco != null && endereco.cd_loc_cidade > 0)
                endereco.bairros = DataAccessLoc.getBairroPorCidade(endereco.cd_loc_cidade, 0).ToList();
            return endereco;
        }

        public EnderecoSGF verificaSeExisteEnderecoOuGravar(EnderecoSGF endereco)
        {
            EnderecoSGF enderecoBase = new EnderecoSGF();
            //Verificar- Estado
            if (!String.IsNullOrEmpty(endereco.noLocEstado))
            {
                LocalidadeSGF estado = DataAccessEstado.getEstadoBySigla(endereco.noLocEstado);
                if (estado != null)
                {
                    enderecoBase.cd_loc_estado = estado.cd_localidade;
                    enderecoBase.noLocEstado = estado.no_localidade;
                }
                else
                    throw new PessoaBusinessException(string.Format(Componentes.Utils.Messages.Messages.msgNotCadastradoEstadoCep, endereco.noLocEstado), null, 
                        PessoaBusinessException.TipoErro.ERRO_ESTADO_NAO_CADASTRADO_CEP, false);
            }
            //Verificar - Cidade
            if (!String.IsNullOrEmpty(endereco.noLocCidade))
            {
                LocalidadeSGF cidade = DataAccessLoc.getCidadeByNomePorEstado(endereco.noLocCidade, enderecoBase.cd_loc_estado);
                if (cidade == null)
                {
                    LocalidadeSGF localidade = new LocalidadeSGF
                        {
                            cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE,
                            cd_loc_relacionada = enderecoBase.cd_loc_estado,
                            no_localidade = endereco.noLocCidade
                        };
                    cidade = DataAccessLoc.add(localidade, false);
                }
                if (cidade != null)
                {
                    enderecoBase.cd_loc_cidade = cidade.cd_localidade;
                    enderecoBase.noLocCidade = cidade.no_localidade;
                }
            }
            //Verificar - Bairro
            if (!String.IsNullOrEmpty(endereco.noLocBairro) || (!String.IsNullOrEmpty(endereco.noLocRua) && String.IsNullOrEmpty(endereco.noLocBairro)))
            {
                endereco.noLocBairro = String.IsNullOrEmpty(endereco.noLocBairro) ? "" : endereco.noLocBairro;
                LocalidadeSGF bairro = DataAccessLoc.getBairroByNome(endereco.noLocBairro, enderecoBase.cd_loc_cidade, enderecoBase.cd_loc_estado);
                if (bairro == null)
                {
                    LocalidadeSGF localidade = new LocalidadeSGF
                    {
                        cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO,
                        cd_loc_relacionada = enderecoBase.cd_loc_cidade,
                        no_localidade = endereco.noLocBairro
                    };
                    bairro = DataAccessLoc.add(localidade, false);
                }
                if (bairro != null)
                {
                    enderecoBase.cd_loc_bairro = bairro.cd_localidade;
                    enderecoBase.noLocBairro = bairro.no_localidade;
                }
            }

            //Verificar - rua
            if (!String.IsNullOrEmpty(endereco.noLocRua))
            {
                LocalidadeSGF rua = DataAccessLoc.getRuaByNome(endereco.noLocRua, (int)enderecoBase.cd_loc_bairro, enderecoBase.cd_loc_cidade, enderecoBase.cd_loc_estado);
                if (rua == null)
                {
                    LocalidadeSGF localidade = new LocalidadeSGF
                    {
                        cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO,
                        cd_loc_relacionada = enderecoBase.cd_loc_bairro,
                        no_localidade = endereco.noLocRua,
                        dc_num_cep = endereco.num_cep
                    };
                    rua = DataAccessLoc.add(localidade, false);
                }
                if (rua != null)
                {
                    enderecoBase.cd_loc_logradouro = rua.cd_localidade;
                    enderecoBase.noLocRua = rua.no_localidade;
                }
            }

            if (!String.IsNullOrEmpty(endereco.descTipoLogradouro))
            {
                TipoLogradouroSGF tLog = DataAccessTipoLogradouro.findTipoLogradouroByNome(endereco.descTipoLogradouro);
                if (tLog != null)
                {
                    enderecoBase.cd_tipo_logradouro = tLog.cd_tipo_logradouro;
                    enderecoBase.descTipoLogradouro = tLog.no_tipo_logradouro;
                }
            }
            if(enderecoBase != null && enderecoBase.cd_loc_cidade > 0)
                enderecoBase.bairros = DataAccessLoc.getBairroPorCidade(enderecoBase.cd_loc_cidade, 0).ToList();

            return enderecoBase;
        }

        #endregion

		#region Bairro

			public IEnumerable<LocalidadeSGF> GetAllBairro()
			{
				return DataAccessLoc.GetAllBairro();
			}

            public IEnumerable<LocalidadeSGF> GetBairroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade)
			{
                IEnumerable<LocalidadeSGF> retorno = new List<LocalidadeSGF>();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
                {
                    // Reajusta os parâmetros de ordenação da pesquisa:
				    if (parametros.sort == null)
					    parametros.sort = "no_localidade";

				    retorno = DataAccessLoc.GetBairroSearch(parametros, descricao, inicio, cd_cidade);
                    transaction.Complete();
                }
                return retorno;
			}

			public LocalidadeSGF GetBairroById(int id)
			{
				return DataAccessLoc.GetBairroById(id);
			}

			public IEnumerable<LocalidadeSGF> FindBairro(string searchText)
			{
				return DataAccessLoc.FindBairro(searchText);
			}

			/// <summary>
			///Retorna um bairro ou cadastra o bairro caso não exista na base
			/// </summary>
			/// <param name="descricao"></param>
			/// <returns></returns>
			public LocalidadeSGF GetBairroDesc(string descricao)
			{

				var bairro = DataAccessLoc.GetBairroDesc(descricao);
				if (bairro == null)
				{
					return AdcionaLocalidade(descricao, BAIRRO, null);
				}
				else
				{
					return bairro;
				}
			   
			}

            public IEnumerable<LocalidadeSGF> getBairroPorCidade(int cd_cidade, int cd_bairro)
            {
                return DataAccessLoc.getBairroPorCidade(cd_cidade, cd_bairro); 
            }

		#endregion

		#region Distrito

			public IEnumerable<LocalidadeSGF> GetAllDistrito()
			{
				return DataAccessLoc.GetAllDistrito();
			}

            public IEnumerable<LocalidadeSGF> GetDistritoSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade)
			{
                IEnumerable<LocalidadeSGF> retorno = new List<LocalidadeSGF>();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
                {
                    // Reajusta os parâmetros de ordenação da pesquisa:
				    if (parametros.sort == null)
					    parametros.sort = "no_localidade";

				    retorno = DataAccessLoc.GetDistritoSearch(parametros, descricao, inicio, cd_cidade);
                    transaction.Complete();
                }
                return retorno;
			}

			public IEnumerable<LocalidadeSGF> FindDistrito(string searchText)
			{
				return DataAccessLoc.FindDistrito(searchText);
			}

			public LocalidadeSGF GetDistritoById(int id)
			{
				return DataAccessLoc.GetDistritoById(id);
			}

		#endregion

		#region Cidade

		  public IEnumerable<LocalidadeSGF> GetAllCidade()
		  {
			  return DataAccessLoc.GetAllCidade();
		  }

          public IEnumerable<CidadeUI> GetCidadeSearch(SearchParameters parametros, string descricao, bool inicio, int nmMunicipio, int cdEstado)
		  {
              IEnumerable<CidadeUI> retorno = new List<CidadeUI>();
              using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
              {
                 // Reajusta os parâmetros de ordenação da pesquisa:
			      if (parametros.sort == null)
				      parametros.sort = "no_localidade";
		  
			      retorno = DataAccessLoc.GetCidadeSearch(parametros, descricao, inicio, nmMunicipio, cdEstado);
                  transaction.Complete();
              }
              return retorno;
		  }

		  public IEnumerable<LocalidadeSGF> FindCidade(string searchText)
		  {
			  return DataAccessLoc.FindCidade(searchText);
		  }

		  public LocalidadeSGF GetCidadeById(int id)
		  {
			  return DataAccessLoc.GetCidadeById(id);
		  }

		  public IEnumerable<LocalidadeSGF> GetAllCidade(int idEstado)
		  {
			  return DataAccessLoc.GetAllCidade(idEstado);
		  }

          public IEnumerable<LocalidadeSGF> GetAllCidade(string search, int idEstado)
          {
              return DataAccessLoc.GetAllCidade(search, idEstado);
          }

		  /// <summary>
		  /// Adciona uma cidade 
		  /// </summary>
		  /// <param name="cidade"></param>
		  /// <returns></returns>
          public CidadeUI PostCidade(CidadeUI cidade)
		  {
              LocalidadeSGF localidade = new LocalidadeSGF
              {
                  cd_localidade = cidade.cd_localidade,
                  cd_tipo_localidade = CIDADE,
                  cd_loc_relacionada = cidade.cd_loc_relacionada,
                  no_localidade = cidade.no_localidade,
                  nm_municipio = cidade.nm_municipio
              };
              var locAdd = DataAccessLoc.add(localidade, false);
              cidade.cd_localidade = locAdd.cd_localidade;
              return cidade;
		  }
          public CidadeUI PutCidade(CidadeUI cidade)
          {
              LocalidadeSGF localidade = new LocalidadeSGF
              {
                  cd_localidade = cidade.cd_localidade,
                  cd_tipo_localidade = CIDADE,
                  cd_loc_relacionada = cidade.cd_loc_relacionada,
                  no_localidade = cidade.no_localidade,
                  nm_municipio = cidade.nm_municipio
              };

              var putlocalidade = PutLocalidade(localidade);

              return cidade;
          }

		  public IEnumerable<LocalidadeUI> GetCidadePaisEstado(SearchParameters parametros, int pais, int estado, int numMunicipio, string desCidade)
		  {
              IEnumerable<LocalidadeUI> retorno = new List<LocalidadeUI>();
              using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
              {

                  if (parametros.sort == null)
                      parametros.sort = "no_cidade";
                  retorno = DataAccessLoc.GetCidadePaisEstado(parametros, pais, estado, numMunicipio, desCidade);
                  transaction.Complete();
              }
              return retorno;
		  }


	   #endregion

		#region Localidade

          public LocalidadeSGF PostLocalidade(LocalidadeSGF localidade)
          {
              DataAccessLoc.add(localidade, false);
              return localidade;
          }

		public LocalidadeSGF PutLocalidade(LocalidadeSGF localidade)
		{
			DataAccessLoc.edit(localidade, false);
			return localidade;
		}

        public bool DeleteLocalidade(List<LocalidadeSGF> localidades)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccessLoc.deleteAll(localidades);
                deleted = true;
                transaction.Complete();
            }
            return deleted;
        }

		#endregion

		#region Pais

        public IEnumerable<PaisUI> GetPaisEstado()
		{
            return DataAccessPais.getPaisEstado();
		}

        public IEnumerable<PaisUI> GetAllPais()
        {
            return DataAccessPais.GetAllPais();
        }

        public IEnumerable<PaisUI> GetAllPaisPorSexoPessoa()
        {
            return DataAccessPais.GetAllPaisPorSexoPessoa();
        }

		public IEnumerable<PaisUI> GetPaisSearch(SearchParameters parametros, string descricao, bool inicio)
		{
            IEnumerable<PaisUI> retorno = new List<PaisUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "dc_pais";

			    retorno = DataAccessPais.GetPaisSearch(parametros, descricao, inicio);
                transaction.Complete();
            }
            return retorno;
		}      

		public IEnumerable<PaisUI> FindPais(string searchText)
		{
			return DataAccessPais.FindPais(searchText);
		}

		public PaisUI GetPaisById(int id)
		{
			return DataAccessPais.GetPaisById(id);
		}

        public PaisUI PostPaisLocalidade(PaisUI paisUI)
		{
			PaisSGF pais = new PaisSGF
			{
				// cd_localidade_pais = postlocalidade.cd_localidade,
				dc_num_pais = paisUI.dc_num_pais,
				dc_nacionalidade_masc = paisUI.dc_nacionalidade_masc,
				dc_nacionalidade_fem = paisUI.dc_nacionalidade_fem,
				sg_pais = paisUI.sg_pais,
				dc_pais = paisUI.dc_pais
				//PaisLocalidade = localidade

			};
			LocalidadeSGF localidade = new LocalidadeSGF
			{
				cd_localidade = paisUI.cd_localidade,
				cd_tipo_localidade = paisUI.cd_tipo_localidade,
				no_localidade = paisUI.dc_pais,
				Pais = pais
			};
			LocalidadeSGF postlocalidade = new LocalidadeSGF();
			using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
			{
				postlocalidade = PostLocalidade(localidade);
			   
				//DataAccessPais.add(pais, false);
				transaction.Complete();
			}
            PaisUI pUI= new PaisUI();
            string dc_pais = paisUI.dc_pais;
            string sg_pais = paisUI.sg_pais;
            string dc_nacionalidade_fem = paisUI.dc_nacionalidade_fem;
            string dc_nacionalidade_masc = paisUI.dc_nacionalidade_masc;
            string dc_num_pais = paisUI.dc_num_pais;
            pUI = PaisUI.fromPais(postlocalidade, dc_pais, sg_pais, dc_nacionalidade_fem, dc_nacionalidade_masc, dc_num_pais);

            return pUI;
		}

		public PaisUI PutPaisLocalidade(PaisUI paisUI)
		{
			PaisSGF pais = new PaisSGF
			{
				cd_localidade_pais = paisUI.cd_localidade,
				dc_num_pais = paisUI.dc_num_pais,
				dc_nacionalidade_masc = paisUI.dc_nacionalidade_masc,
				dc_nacionalidade_fem = paisUI.dc_nacionalidade_fem,
				sg_pais = paisUI.sg_pais,
				dc_pais = paisUI.dc_pais
			};
			LocalidadeSGF localidade = new LocalidadeSGF
			{
				cd_localidade = paisUI.cd_localidade,
				cd_tipo_localidade = paisUI.cd_tipo_localidade,
				no_localidade = paisUI.dc_pais,
				Pais = pais
			};
		   
			using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
			{
				var putlocalidade = PutLocalidade(localidade);
				DataAccessPais.edit(pais, false);
				transaction.Complete();
			}
			return paisUI;
		}

        public bool DeletePais(List<PaisUI> paises)
        {
            // var cdsLoc =  DataAccessPais.GetPaisesLocalidades(paises);
            List<LocalidadeSGF> locs = new List<LocalidadeSGF>();
            LocalidadeSGF loc = new LocalidadeSGF();
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                for (int i = 0; i < paises.Count; i++)
                {
                    loc = DataAccessLoc.GetLocalidade(paises[i].cd_localidade);
                    locs.Add(loc);
                }
                ret = DataAccessPais.deleteAll(paises);
                DeleteLocalidade(locs);
                transaction.Complete();
            }
            return ret;

		}

		#endregion

		#region endereço

		public EnderecoSGF PostEndereco(EnderecoSGF endereco)
		{
			return DataAccessEnd.add(endereco, false);
		}

		public EnderecoSGF EditEndereco(EnderecoSGF endereco)
		{
			return DataAccessEnd.edit(endereco, false);
		}
        
        public EnderecoSGF saveChangesEndereco(EnderecoSGF endereco)
        {
            DataAccessEnd.saveChanges(false);
            return endereco;
        }
        
        public bool deleteEndereco(EnderecoSGF endereco)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccessEnd.delete(endereco, false);
                deleted = true;
                transaction.Complete();
            }
            return deleted;

        }

        public IEnumerable<EnderecoSGF> GetAllEnderecoByPessoa(int cdPessoa, int cd_endereco)
        {
            return DataAccessEnd.GetAllEnderecoByPessoa(cdPessoa, cd_endereco);
        }

        public EnderecoSGF getEnderecoResponsavelCPF(int cd_pessoa)
        {
            return DataAccessEnd.getEnderecoResponsavelCPF(cd_pessoa);
        }

        public EnderecoSGF getEnderecoByCdEndereco(int cd_endereco)
        {
            return DataAccessEnd.getEnderecoByCdEndereco(cd_endereco);
        }

        #endregion

        #region Estado

        public IEnumerable<EstadoUI> GetAllEstado()
		{
			return DataAccessEstado.GetAllEstado();
		}

        public IEnumerable<EstadoUI> GetEstadoEstado()
        {
            return DataAccessEstado.getEstadoEstado();
        }

		public IEnumerable<EstadoUI> GetEstadoSearch(SearchParameters parametros, string descricao, bool inicio, int cdPais)
		{
            IEnumerable<EstadoUI> retorno = new List<EstadoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "no_localidade";

			    retorno = DataAccessEstado.GetEstadoSearch(parametros, descricao, inicio, cdPais);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<EstadoUI> FindEstado(string searchText)
		{
			return DataAccessEstado.FindEstado(searchText);
		}

		public EstadoUI GetEstadoById(int id)
		{
			return DataAccessEstado.GetEstadoById(id);
		}

        public EstadoUI PostEstadoLocalidade(EstadoUI estadoUI)
        {
            EstadoUI est = new EstadoUI();
            EstadoSGF estado = new EstadoSGF
            {
                sg_estado = estadoUI.sg_estado
            };
            LocalidadeSGF postlocalidade = new LocalidadeSGF();
            LocalidadeSGF localidade = new LocalidadeSGF
            {
                cd_localidade = estadoUI.cd_localidade,
                cd_tipo_localidade = estadoUI.cd_tipo_localidade,
                no_localidade = estadoUI.no_localidade,
                cd_loc_relacionada = estadoUI.cd_loc_relacionada,
                Estado = estado
            };

            postlocalidade = PostLocalidade(localidade);
            est = DataAccessEstado.GetEstadoById(postlocalidade.cd_localidade);

            EstadoUI eUI = new EstadoUI();
            string sg_estado = estadoUI.sg_estado;
            eUI = EstadoUI.fromEstado(postlocalidade, sg_estado, estadoUI.no_pais);

            return eUI;
		}
		public EstadoUI PutEstadoLocalidade(EstadoUI estadoUI)
		{
			EstadoSGF estado = new EstadoSGF
			{
				cd_localidade_estado = estadoUI.cd_localidade,
				sg_estado = estadoUI.sg_estado
			};
			LocalidadeSGF localidade = new LocalidadeSGF
			{
				cd_localidade = estadoUI.cd_localidade,
				cd_tipo_localidade = estadoUI.cd_tipo_localidade,
				no_localidade = estadoUI.no_localidade,
                cd_loc_relacionada = estadoUI.cd_loc_relacionada,
				Estado = estado
			};
            
			using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
			{
				var putlocalidade = PutLocalidade(localidade);
				DataAccessEstado.edit(estado, false);
				transaction.Complete();
			}
            return estadoUI;
		}

        public bool DeleteEstado(List<EstadoUI> estados)
        {
            List<LocalidadeSGF> locs = new List<LocalidadeSGF>();
            LocalidadeSGF loc = new LocalidadeSGF();
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                for (int i = 0; i < estados.Count; i++)
                {
                    loc = DataAccessLoc.GetLocalidade(estados[i].cd_localidade);
                    locs.Add(loc);
                }
                ret = DataAccessEstado.deleteAll(estados);
                DeleteLocalidade(locs);
                transaction.Complete();
            }
            return ret;

        }

        public IEnumerable<EstadoUI> getEstadoByPais(int cd_pais)
        {
            return DataAccessEstado.getEstadoByPais(cd_pais);
        }
		
		#endregion

		#region TipoEndereco

		public IEnumerable<TipoEnderecoSGF> GetAllTipoEndereco()
		{
			return DataAccessTpEnd.GetAllTipoEndereco();
		}

		public IEnumerable<TipoEnderecoSGF> GetTipoEnderecoSearch(SearchParameters parametros, string descricao, bool inicio)
		{
            IEnumerable<TipoEnderecoSGF> retorno = new List<TipoEnderecoSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "no_tipo_endereco";

			    retorno = DataAccessTpEnd.GetTipoEnderecoSearch(parametros, descricao, inicio);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<TipoEnderecoSGF> FindTipoEndereco(string searchText)
		{
			return DataAccessTpEnd.FindTipoEndereco(searchText);
		}

		public TipoEnderecoSGF GetTipoEnderecoById(int id)
		{
			return DataAccessTpEnd.GetTipoEnderecoById(id);
		}

		public TipoEnderecoSGF PostTipoEndereco(TipoEnderecoSGF tipoEndereco)
		{
			DataAccessTpEnd.add(tipoEndereco, false);
			return tipoEndereco;
		}
		public TipoEnderecoSGF PutTipoEndereco(TipoEnderecoSGF tipoEndereco)
		{
            if (tipoEndereco.cd_tipo_endereco >= 1 && tipoEndereco.cd_tipo_endereco <= 8)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

			DataAccessTpEnd.edit(tipoEndereco, false);
			return tipoEndereco;
		}
        public bool DeleteTipoEndereco(List<TipoEnderecoSGF> tiposEndereco)
        {
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (TipoEnderecoSGF e in tiposEndereco)
                    if (e.cd_tipo_endereco >= 1 && e.cd_tipo_endereco <= 8)
                        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                ret = DataAccessTpEnd.deleteAll(tiposEndereco);
                transaction.Complete();
            }
            return ret;
        }
        #endregion

		#region ClasseTelefone
		public IEnumerable<ClasseTelefoneSGF> GetAllClasseTelefone()
		{
			return DataAccessClasseTelefone.GetAllClasseTelefone();
		}

		public IEnumerable<ClasseTelefoneSGF> GetClasseTelefoneSearch(SearchParameters parametros, string descricao, bool inicio)
		{
            IEnumerable<ClasseTelefoneSGF> retorno = new List<ClasseTelefoneSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "dc_classe_telefone";

			    retorno = DataAccessClasseTelefone.GetClasseTelefoneSearch(parametros, descricao, inicio);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<ClasseTelefoneSGF> FindClasseTelefone(string searchText)
		{
			return DataAccessClasseTelefone.FindClasseTelefone(searchText);
		}

		public ClasseTelefoneSGF GetClasseTelefoneById(int id)
		{
			return DataAccessClasseTelefone.GetClasseTelefoneById(id);
		}

		public ClasseTelefoneSGF PostClasseTelefone(ClasseTelefoneSGF classeTelefone)
		{
			using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
			{
				DataAccessClasseTelefone.add(classeTelefone, false);
				transaction.Complete();
			}
			return classeTelefone;
		}
		public ClasseTelefoneSGF PutClasseTelefone(ClasseTelefoneSGF classeTelefone)
		{
            if (classeTelefone.cd_classe_telefone >= 1 && classeTelefone.cd_classe_telefone <= 5)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

			DataAccessClasseTelefone.edit(classeTelefone, false);
			return classeTelefone;
		}
        public bool DeleteClasseTelefone(List<ClasseTelefoneSGF> classesTelefone)
		{
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
			{
                foreach (ClasseTelefoneSGF e in classesTelefone)
                    if (e.cd_classe_telefone >= 1 && e.cd_classe_telefone <= 5)
                        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                 ret = DataAccessClasseTelefone.deleteAll(classesTelefone);
                 transaction.Complete();
            }
            return ret;

		}
		#endregion

		#region TipoLogradouro
		public IEnumerable<TipoLogradouroSGF> GetAllTipoLogradouro()
		{
			return DataAccessTipoLogradouro.GetAllTipoLogradouro();
		}

		public IEnumerable<TipoLogradouroSGF> GetTipoLogradouroSearch(SearchParameters parametros, string descricao, bool inicio)
		{
            IEnumerable<TipoLogradouroSGF> retorno = new List<TipoLogradouroSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "no_tipo_logradouro";

			    retorno = DataAccessTipoLogradouro.GetTipoLogradouroSearch(parametros, descricao, inicio);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<TipoLogradouroSGF> FindTipoLogradouro(string searchText)
		{
			return DataAccessTipoLogradouro.FindTipoLogradouro(searchText);
		}

		public TipoLogradouroSGF GetTipoLogradouroById(int id)
		{
			return DataAccessTipoLogradouro.GetTipoLogradouroById(id);
		}

		public TipoLogradouroSGF PostTipoLogradouro(TipoLogradouroSGF tipoLogradouro)
        {
            DataAccessTipoLogradouro.add(tipoLogradouro, false);
            return tipoLogradouro;
        }
		public TipoLogradouroSGF PutTipoLogradouro(TipoLogradouroSGF tipoLogradouro)
		{
            if (tipoLogradouro.cd_tipo_logradouro >= 1 && tipoLogradouro.cd_tipo_logradouro <= 14)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

			DataAccessTipoLogradouro.edit(tipoLogradouro, false);
			return tipoLogradouro;
		}
        public bool DeleteTipoLogradouro(List<TipoLogradouroSGF> tiposLogradouro)
        {
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (TipoLogradouroSGF e in tiposLogradouro)
                    if (e.cd_tipo_logradouro >= 1 && e.cd_tipo_logradouro <= 14)
                        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                ret = DataAccessTipoLogradouro.deleteAll(tiposLogradouro);
                transaction.Complete();
            }
            return ret;
        }
        #endregion

		#region TipoTelefone
		public IEnumerable<TipoTelefoneSGF> GetAllTipoTelefone()
		{
			return DataAccessTipoTelefone.GetAllTipoTelefone();
		}

		public IEnumerable<TipoTelefoneSGF> GetTipoTelefoneSearch(SearchParameters parametros, string descricao, bool inicio)
		{
            IEnumerable<TipoTelefoneSGF> retorno = new List<TipoTelefoneSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "no_tipo_telefone";

			    retorno = DataAccessTipoTelefone.GetTipoTelefoneSearch(parametros, descricao, inicio);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<TipoTelefoneSGF> FindTipoTelefone(string searchText)
		{
			return DataAccessTipoTelefone.FindTipoTelefone(searchText);
		}

		public TipoTelefoneSGF GetTipoTelefoneById(int id)
		{
			return DataAccessTipoTelefone.GetTipoTelefoneById(id);
		}

		public TipoTelefoneSGF PostTipoTelefone(TipoTelefoneSGF tipoTelefone)
		{
            DataAccessTipoTelefone.add(tipoTelefone, false);
			return tipoTelefone;
		}
		public TipoTelefoneSGF PutTipoTelefone(TipoTelefoneSGF tipoTelefone)
		{
            if (tipoTelefone.cd_tipo_telefone >= 1 && tipoTelefone.cd_tipo_telefone <= 9)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

			DataAccessTipoTelefone.edit(tipoTelefone, false);
			return tipoTelefone;
		}
        public bool DeleteTipoTelefone(List<TipoTelefoneSGF> tiposTelefone)
		{
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (TipoTelefoneSGF e in tiposTelefone)
                    if (e.cd_tipo_telefone >= 1 && e.cd_tipo_telefone <= 9)
                        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                DataAccessTipoTelefone.deleteAll(tiposTelefone);
                deleted = true;
                transaction.Complete();
            }
            return deleted;
		}
		#endregion

		#region Operadora
		public IEnumerable<Operadora> GetAllOperadora()
		{
			return DataAccessOperadora.GetAllOperadora();
		}

        public IEnumerable<Operadora> GetAllOperadorasAtivas(int? cd_operadora) {
            return DataAccessOperadora.GetAllOperadorasAtivas(cd_operadora);
        }

		public IEnumerable<Operadora> GetOperadoraSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativa)
		{
            IEnumerable<Operadora> retorno = new List<Operadora>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
			    if (parametros.sort == null)
				    parametros.sort = "no_operadora";
			    parametros.sort = parametros.sort.Replace("operadora_ativa", "id_operadora_ativa");

			    retorno = DataAccessOperadora.GetOperadoraSearch(parametros, descricao, inicio, ativa);
                transaction.Complete();
            }
            return retorno;
		}

		public IEnumerable<Operadora> FindOperadora(string searchText)
		{
			return DataAccessOperadora.FindOperadora(searchText);
		}

		public Operadora GetOperadoraById(int id)
		{
			return DataAccessOperadora.GetOperadoraById(id);
		}

		public Operadora PostOperadora(Operadora operadora)
		{
			DataAccessOperadora.add(operadora, false);
			return operadora;
		}
		public Operadora PutOperadora(Operadora operadora)
		{
			DataAccessOperadora.edit(operadora, false);
			return operadora;
		}
        public bool DeleteOperadora(List<Operadora> operadoras)
        {
            bool ret = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                ret = DataAccessOperadora.deleteAll(operadoras);

                transaction.Complete();
            }
            return ret;
		}
		#endregion

        #region Atividade
        public IEnumerable<Atividade> GetAllAtividade()
        {
            return DataAccessAtividade.findAll(false);
        }

        public IEnumerable<Atividade> GetAtividadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int natureza, string cnae)
        {
            IEnumerable<Atividade> retorno = new List<Atividade>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_atividade";
                parametros.sort = parametros.sort.Replace("atividade_ativa", "id_atividade_ativa");
                parametros.sort = parametros.sort.Replace("natureza_atividade", "id_natureza_atividade");

                retorno = DataAccessAtividade.GetAtividadeSearch(parametros, descricao, inicio, status, natureza, cnae);
                transaction.Complete();
            }
            return retorno;
        }

        //public IEnumerable<AtividadePessoa> FindOperadora(string searchText)
        //{
        //    return DataAccessOperadora.FindOperadora(searchText);
        //}

        public Atividade GetAtividadeById(int id)
        {
            return DataAccessAtividade.findById(id, false);
        }

        public Atividade PostAtividade(Atividade atividadePessoa)
        {
            DataAccessAtividade.add(atividadePessoa, false);
            return atividadePessoa;
        }
        public Atividade PutAtividade(Atividade atividadePessoa)
        {
            DataAccessAtividade.edit(atividadePessoa, false);
            return atividadePessoa;
        }
        public bool DeleteAtividade(List<Atividade> atividadePessoa)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccessAtividade.deleteAll(atividadePessoa);
                deleted = true;
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<Atividade> getAllListAtividades(string searchText, int natureza, bool? status)
        {
            return DataAccessAtividade.getAllListAtividades(searchText, natureza, status);
        }
        #endregion

		#region Logradouro

		public bool existsLocalidade(int cdLoc, string no_rua)
		{
			return DataAccessLoc.existsLocalidade(cdLoc, no_rua);
		}

		/// <summary>
		/// Retorna um endereço do tipo rua ou cadastra a rua caso não exista na base
		/// </summary>
		/// <param name="descricao"></param>
		/// <returns></returns>
		public LocalidadeSGF GetEnderecoDesc(string descricao)
		{
			LocalidadeSGF rua = DataAccessLoc.GetEnderecoDesc(descricao);
			if (rua == null)
			{
				return AdcionaLocalidade(descricao, RUA, null);
			}
			else
			{
				return rua;
			}
		}

		private LocalidadeSGF AdcionaLocalidade(string descricao, byte tipo, int? localidadeRel)
		{
			return DataAccessLoc.add(new LocalidadeSGF
			{
				no_localidade = descricao,
				cd_tipo_localidade = tipo,
				cd_loc_relacionada = localidadeRel
			}, false);
		}
		// Tras a rua pela descrição digitada pelo usuario
		public IEnumerable<LocalidadeSGF> GetAllEndereco(string searchText)
		{
			return DataAccessLoc.GetAllEndereco(searchText);
		}

        public IEnumerable<LocalidadeSGF> getLogradouroSearch(SearchParameters parametros, string descricao, bool inicio,int cd_estado, int cd_cidade, int cd_bairro, string cep)
        {
            IEnumerable<LocalidadeSGF> retorno = new List<LocalidadeSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_localidade";
                parametros.sort = parametros.sort.Replace("no_localidade_bairro", "LocalidadeRelacionada.no_localidade");
                parametros.sort = parametros.sort.Replace("no_localidade_cidade", "LocalidadeRelacionada.LocalidadeRelacionada.no_localidade");
                retorno = DataAccessLoc.getLogradouroSearch(parametros, descricao, inicio, cd_estado, cd_cidade, cd_bairro, cep);
                transaction.Complete();
            }
            return retorno;
        }

        public LocalidadeSGF addLogradouro(LocalidadeSGF logradouro)
        {
            DataAccessLoc.add(logradouro, false);
            return logradouro;
        }
        public LocalidadeSGF editLogradouro(LocalidadeSGF logradouro)
        {
            LocalidadeSGF localidadeContext = DataAccessLoc.findById(logradouro.cd_localidade, false);
            localidadeContext = LocalidadeSGF.ChangeValuesLogradouro(localidadeContext, logradouro);
            DataAccessLoc.saveChanges(false);
            return logradouro;
        }

        public bool deleteLogradouros(int[] cdLogradouros)
        {
            bool retorno = false;

            if (cdLogradouros != null && cdLogradouros.Count() > 0)
            {
                List<LocalidadeSGF> logsContext = DataAccessLoc.getLogradouros(cdLogradouros).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (LocalidadeSGF l in logsContext)
                        retorno = DataAccessLoc.delete(l, false);
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public IEnumerable<LocalidadeSGF> getAllLogradouroPorBairro(int cd_bairro)
        {
            return DataAccessLoc.getAllLogradouroPorBairro(cd_bairro);
        }

        public IEnumerable<LogradouroCEP> getLogradouroCorreio(string descricao, string estado, string cidade, string bairro, string cep, int? numero)
        {
            IEnumerable<LogradouroCEP> retorno = new List<LogradouroCEP>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessLoc.getLogradouroCorreio(descricao, estado, cidade, bairro, cep, numero).ToList();
                transaction.Complete();
            }
            return retorno;
        }

		#endregion        
    
    }
}

