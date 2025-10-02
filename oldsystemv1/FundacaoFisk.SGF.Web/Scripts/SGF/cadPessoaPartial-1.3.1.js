// Constantes
var LOGRADOURO = 1, TIPOLOGRADOURO = 2;
var codCidade = null;
var codCidadeDialog = null;
var codCidadeRelac = null;
var natureza = 0;
var AUTOCOMPLETE_FECHADO = 0, AUTOCOMPLETE_ABERTO = 1, CELULAR = 3, TELEFONE = 1,EMAIL = 4;
var PESSOAJURIDICA = 2, PESSOAFISICA = 1, CARGO = 3;

var MASCULINO = 2, FEMININO = 1, NAO_BINARIO = 3, PREFIRO_NAO_RESPONDER_NEUTRO = 4,  RELACIONAMENTOPESSOA = 2, USARCPF = 3, CPFRELAC = 6;
var PAPEL_RESPONSAVEL = 3;
var tagMensagem = "";
var MEGAEMBYTES = 1048576;

var EnumPapelRelacionamento =
{
    RESPONSAVEL_FINANCEIRO: 3,
    ALUNO_RESPONSAVEL: 9,
}

//Monta Máscara para um campo de data 

function cleanUsarCpf() {
    try {
        dojo.byId("cdPessoaCpf").value = "";
        dojo.byId("nomPessoaCpf").value = "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearCidade(campoId, valorDefault, campoDescricao) {
    try {
        $("#" + campoId).val(0);
        $("#" + valorDefault).val("");
        $("#" + campoDescricao).val("");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function enviarEmail(idElement) {
    try {
        if (hasValue(dojo.byId(idElement).value)) {
            var body = escape("Digite a mensagem a ser enviada."); // salta a  linha String.fromCharCode(13) 
            var emailTo = hasValue(dojo.byId(idElement).value != "") ? dojo.byId(idElement).value : '';
            window.location.href = "mailto:" + emailTo + "?body=" + body;
        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgNecInclEmail);
            apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Monta máscaras para telefone, cpf e cnpj
function inicial() {
    require([
           "dojo/ready",
           "dojo/on",
           "dojo/store/Memory",
           "dijit/form/FilteringSelect"
    ], function (ready, on, Memory, FilteringSelect) {
        ready(function () {
            try {
                $('#celular').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $('#celularRelac').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $("#telefone").mask("(99) 9999-9999");
                $("#telefonePessoaRelac").mask("(99) 9999-9999");
                $("#telefoneRelac").mask("(99) 9999-9999");
                $("#cpf").mask("999.999.999-99");
                $("#cnpj").mask("99.999.999/9999-99");
                $("#cep").mask("99999-999");
                $("#cepOutrosEnd").mask("99999-999");
                $("#cepRelac").mask("99999-999");
                $("#dc_num_cnpj_cnab").mask("99.999.999/9999-99");
                maskHHMMSS("#horaCadastro");

                //Monta o tipo de Sociedade(Limitada/Anônima)
                var sociedade = new Array(
                { name: "limitada", id: 1 },
                { name: "anônima", id: 2 }
                );
                criarOuCarregarCompFiltering('tipoSociedade', sociedade, 'width: 75px;', 1, ready, Memory, FilteringSelect, 'id', 'name', null);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadEditPessoaJuridica(dataPessoaJuridica, carregando) {
    try {
        setarValuePessoaJuridica(dataPessoaJuridica.retorno, carregando);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEditPessoaFisica(dataPessoaFisica, carregando) {
    try {
        setarValuePessoaFisica(dataPessoaFisica.retorno, carregando);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGrupoEstoqueMaterial(items, linkGrupo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var itemsCb = [];
                var cbGrupo = dijit.byId(linkGrupo);
                Array.forEach(items, function (value, i) {
                    itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
                });
                var stateStore = new Memory({
                    data: itemsCb
                });
                cbGrupo.store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function showPessoaFK(natureza, cdPessoa, tipoPapel, pFuncaoRetorno) {
    showCarregando();
    require([
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/ready",
    "dojox/json/ref",
    "dijit/Dialog",
    "dojo/domReady!"
    ], function (xhr, Memory, ready, ref) {
        ready(function () {
            try {
                toggleDisabled("dtaCadastro", false);
                toggleDisabled("horaCadastro", false);
                apresentaMensagem(dojo.byId("descApresMsg").value, null);
                limparCadPessoaFK();
                if (hasValue(natureza))
                    dijit.byId("naturezaPessoa").set("value", natureza);
                else {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não foi possível carregar os dados.");
                    apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                    return false;
                }
                if (cdPessoa != null && cdPessoa > 0) {
                    toggleDisabled("dtaCadastro", true);
                    toggleDisabled("horaCadastro", true);
                    if (natureza == PESSOAJURIDICA) {
                        xhr.get({
                            url: Endereco() + "/api/aluno/VerificarExistEmpresaByCnpjOrCdEmpresa?cnpj=&cdEmpresa=" + parseInt(cdPessoa),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataPessoaJuridica) {
                            try {
                                dataPessoaJuridica = jQuery.parseJSON(dataPessoaJuridica);
                                var cd_oper = getOperadora(dataPessoaJuridica);
                                xhr.post({
                                    url: Endereco() + "/api/pessoa/ComponentesPessoa?tipo=" + tipoPapel + "&cd_operadora=" + cd_oper,// tipo papel
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                    postData: ref.toJson(tipoPapel)
                                }).then(function (comPessoaUI) {
                                    try {
                                        comPessoaUI = jQuery.parseJSON(comPessoaUI);
                                        loadComponentesTela(comPessoaUI.retorno.tiposEndereco, comPessoaUI.retorno.tiposLogradouro, comPessoaUI.retorno.estadosUI, comPessoaUI.retorno.orgaosExpedidores,
                                            comPessoaUI.retorno.classesTelefone, comPessoaUI.retorno.tiposTelefone, comPessoaUI.retorno.papeis, comPessoaUI.retorno.operadoras,
                                            comPessoaUI.retorno.qualifRelacionamentos, Memory, false);
                                        loadEditPessoaJuridica(dataPessoaJuridica, true);

                                        if (dijit.byId("cb_grupo_estoque_material") != null && dijit.byId("cb_grupo_estoque_material") != undefined &&  hasValue(comPessoaUI.retorno.gruposEstoques)) {
                                            loadGrupoEstoqueMaterial(comPessoaUI.retorno.gruposEstoques, "cb_grupo_estoque_material");
                                        }

                                        if (hasValue(pFuncaoRetorno, false))
                                            pFuncaoRetorno.call();
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                }, function (error) {
                                    apresentaMensagem(dojo.byId("descApresMsg").value, error);
                                    showCarregando();
                                });
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        },
                         function (error) {
                             apresentaMensagem(dojo.byId("descApresMsg").value, error);
                             showCarregando();
                         });
                    }
                    else {
                        xhr.get({
                            url: Endereco() + "/api/aluno/VerificarExistPessoByCpfOrCdPessoa?cpf=&cdPessoa=" + parseInt(cdPessoa),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataPessoaFisica) {
                            try {
                                dataPessoaFisica = jQuery.parseJSON(dataPessoaFisica);
                                var cd_oper = getOperadora(dataPessoaFisica);
                                xhr.post({
                                    url: Endereco() + "/api/pessoa/ComponentesPessoa?tipo=" + tipoPapel + "&cd_operadora=" + cd_oper,// tipo papel
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                    postData: ref.toJson(tipoPapel)
                                }).then(function (comPessoaUI) {
                                    try {
                                        comPessoaUI = jQuery.parseJSON(comPessoaUI);
                                        loadComponentesTela(comPessoaUI.retorno.tiposEndereco, comPessoaUI.retorno.tiposLogradouro, comPessoaUI.retorno.estadosUI, comPessoaUI.retorno.orgaosExpedidores,
                                        comPessoaUI.retorno.classesTelefone, comPessoaUI.retorno.tiposTelefone, comPessoaUI.retorno.papeis, comPessoaUI.retorno.operadoras,
                                        comPessoaUI.retorno.qualifRelacionamentos, Memory, false);
                                        loadEditPessoaFisica(dataPessoaFisica, true);

                                        if (dataPessoaFisica.retorno.pessoaFisica['PessoaRaf'] != null &&
                                            dataPessoaFisica.retorno.pessoaFisica['PessoaRaf'] != undefined) {
                                            loadAlunoRaf(dataPessoaFisica.retorno.pessoaFisica.PessoaRaf);
                                        }
                                        
                                        if (dijit.byId("cb_grupo_estoque_material") != null && dijit.byId("cb_grupo_estoque_material") != undefined && hasValue(comPessoaUI.retorno.gruposEstoques)) {
                                            loadGrupoEstoqueMaterial(comPessoaUI.retorno.gruposEstoques, "cb_grupo_estoque_material");
                                        }

                                        if (hasValue(pFuncaoRetorno, false))
                                            pFuncaoRetorno.call();
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                }, function (error) {
                                    apresentaMensagem(dojo.byId("descApresMsg").value, error);
                                    showCarregando();
                                });
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        },
                         function (error) {
                             apresentaMensagem(dojo.byId("descApresMsg").value, error);
                             showCarregando();
                         });
                    }
                }
                else {
                    xhr.post({
                        url: Endereco() + "/api/pessoa/ComponentesPessoa?tipo=" + tipoPapel + "&cd_operadora=null",// tipo papel
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        postData: ref.toJson(tipoPapel)
                    }).then(function (comPessoaUI) {
                        try {
                            comPessoaUI = jQuery.parseJSON(comPessoaUI);
                            loadComponentesTela(comPessoaUI.retorno.tiposEndereco, comPessoaUI.retorno.tiposLogradouro, comPessoaUI.retorno.estadosUI, comPessoaUI.retorno.orgaosExpedidores,
                                comPessoaUI.retorno.classesTelefone, comPessoaUI.retorno.tiposTelefone, comPessoaUI.retorno.papeis, comPessoaUI.retorno.operadoras,
                                comPessoaUI.retorno.qualifRelacionamentos, Memory, true);
                            observeTitlePaneOpenUpdate();

                            if (dijit.byId("cb_grupo_estoque_material") != null && dijit.byId("cb_grupo_estoque_material") != undefined && hasValue(comPessoaUI.retorno.gruposEstoques)) {
                                loadGrupoEstoqueMaterial(comPessoaUI.retorno.gruposEstoques, "cb_grupo_estoque_material");
                            }

                            if (hasValue(pFuncaoRetorno, false))
                                pFuncaoRetorno.call();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }, function (error) {
                        apresentaMensagem(dojo.byId("descApresMsg").value, error);
                        showCarregando();
                    });
                }

                if (dijit.byId("naturezaPessoa").value == PESSOAFISICA)
                    dojo.byId("labelAtividade").innerHTML = "Profissão:";
                else
                    dojo.byId("labelAtividade").innerHTML = "Atividade Principal:";

            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function habilitaOperadora(obj) {
    try {
        if (hasValue(obj.value) && obj.value != '(__) ____-_____' && obj.value != '(__) _____-____' && (obj.value.length > 2 && obj.value.substring(obj.value.length - 2, obj.value.length) != '__'))
            dijit.byId('operadora').set('disabled', false);
        else {
            dijit.byId('operadora').set('disabled', true);
            dijit.byId('operadora').set('value', '');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getOperadora(dataPessoaFisica) {
    try {
        if (hasValue(dataPessoaFisica) && hasValue(dataPessoaFisica.retorno) && hasValue(dataPessoaFisica.retorno.contatosUI) && hasValue(dataPessoaFisica.retorno.contatosUI.contatosPrincipais))
            for (var i = 0; i < dataPessoaFisica.retorno.contatosUI.contatosPrincipais.length; i++)
                if (dataPessoaFisica.retorno.contatosUI.contatosPrincipais[i].cd_tipo_telefone == CELULAR)
                    return dataPessoaFisica.retorno.contatosUI.contatosPrincipais[i].cd_operadora;
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadComponentesTela(dataTipoEndereco, dataLogradouro, dataEstado, dataOrgaoExpedidor, dataClasseTelefone, dataTipoTelefone, dataPapel, dataOperadora, qualifRelacionamentos, Memory, carregando) {
    try {
        loadTipoEndereco(dataTipoEndereco, 'tipoEndereco', Memory);
        loadOperadora(dataOperadora, 'operadora', Memory);
        loadOperadora(dataOperadora, 'operadoraDialog', Memory);
        loadTipoEndereco(dataTipoEndereco, 'tipoEnderecoOutrosEnd', Memory);
        //loadTipoEndereco(dataTipoEndereco, 'tipoEnderecoRelac', Memory);
        loadLogradouro(dataLogradouro, 'logradouro', Memory);
        loadLogradouro(dataLogradouro, 'tipoLogradouroOutrosEnd', Memory);
        //loadLogradouro(dataLogradouro, 'logradouroRelac', Memory);
        loadEstado(dataEstado, 'estado', Memory);
        loadEstado(dataEstado, 'estadoOutrosEnd', Memory);
        //loadEstado(dataEstado, 'estadoRelac', Memory);
        loadEstaOrgaoExpedidor(dataEstado, Memory);
        loadOrgaoExpedidor(dataOrgaoExpedidor, Memory);
        loadSexo('sexo', Memory);
        loadSexo('sexoRelac', Memory);
        loadClasseTelefone(dataClasseTelefone, Memory);
        loadTipoTelefone(dataTipoTelefone, Memory);
        loadDropDownRelac(dataPapel);
        criarOuCarregarCompFiltering("grauParent", qualifRelacionamentos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_qualif_relacionamento', 'no_qualif_relacionamento');
        if (hasValue(carregando) && carregando)
            showCarregando();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataPessoaBySexo(sexo, idEstadoCivil, idTratamento, idNacionalidade) {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/pessoa/getAllEstadoCivil/", // Estado Civil
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataEstadoCivil) {
            try {
                dataEstadoCivil = jQuery.parseJSON(dataEstadoCivil);
                dojo.xhr.get({
                    url: Endereco() + "/api/pessoa/getAllTratamentoPessoa/", // Tratamento Pessoa
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataTratamentoPessoa) {
                    try {
                        dataTratamentoPessoa = jQuery.parseJSON(dataTratamentoPessoa);
                        dojo.xhr.get({
                            url: Endereco() + "/api/localidade/GetAllPaisPorSexoPessoa/", // Estado Civil
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataPais) {
                            try {
                                loadEstadoCivil(dataEstadoCivil, sexo, idEstadoCivil);
                                loadTratamentoPessoa(dataTratamentoPessoa, sexo, idTratamento);
                                loadNacionalidade(dataPais, sexo, idNacionalidade);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, function (error) {
                            apresentaMensagem(dojo.byId("descApresMsg").value, error);
                        });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem(dojo.byId("descApresMsg").value, error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem(dojo.byId("descApresMsg").value, error);
        });

    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroPessoaFK(permissoes) {
    //Criação da Grade de sala
    require([
    "dijit/registry",
    "dojox/grid/EnhancedGrid",
    "dojo/data/ObjectStore",
    "dojo/store/Memory",
    "dojo/query",
    "dijit/form/Button",
    "dojo/ready",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dojo/on",
    "dojox/json/ref",
    "dojox/grid/enhanced/plugins/Pagination"
    ], function (registry, EnhancedGrid, ObjectStore, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, on, ref, Pagination) {
        ready(function () {
            try {
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;

                var gridContato = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "Tipo", field: "des_tipo_contato", width: "50px", styles: "text-align: left; min-width:50px; max-width:50px;" },
                        { name: "Classe", field: "no_classe", width: "50px", styles: "text-align: left; min-width:50px; max-width:50px;" },
                        { name: "Contato", field: "dc_fone_mail", width: "30%" }//,
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridContatos");
                gridContato.startup();
                gridContato.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        apresentaMensagem(dojo.byId("descApresMsg").value, null);
                        IncluirAlterar(0, 'divAlterarOutrosContatos', 'divIncluirOutrosContatos', 'divExcluirOutrosContatos', dojo.byId("descApresMsg").value, 'divCancelarOutrosContatos', 'divClearOutrosContatos');
                        limparOutrosContatos();
                        keepVelusOutrosContatos();
                        dijit.byId("dialogContato").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //Criação da grade Outros Endereços
                var gridEndereco = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "CEP", field: "num_cep", width: "100px" },
                        { name: "Logradouro", field: "noLocRua", width: "50%" },
                        { name: "Número", field: "dc_num_endereco", width: "100px", styles: "text-align: right" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridEnderecos");
                gridEndereco.startup();
                gridEndereco.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        apresentaMensagem(dojo.byId("descApresMsg").value, null);
                        IncluirAlterar(0, 'divAlterarOutrosEnd', 'divIncluirOutrosEnd', 'divExcluirOutrosEnd', dojo.byId("descApresMsg").value, 'divCancelarOutrosEnd', 'divCleanOutrosEnd');
                        limparOutrosEnderecos();
                        keepVelusOutrosEnderecos();
                        dijit.byId("DialogEndereco").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //Criação da grade Outros Relacionamentos
                var gridRelacionamento = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: " ", field: "ehSelecionado", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxRelac },
                        { name: "Pessoa", field: "no_pessoa", width: "50%" },
                        { name: "Parentesco", field: "desc_qualif_relacionamento", width: "24%" },
                        { name: "Telefone", field: "dc_fone_mail", width: "24%" },
                        { name: "Relação", field: "no_papel", width: "26%" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["5", "10", "20", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "5",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridRelacionamentos");
                gridRelacionamento.startup();

                //Cria o combo com a naruteza da pessoa para o cadastro.
                loadNaturezaPessoaCadastro(Memory);
                //Criação dos botoes CadPessoaFK
                new Button({ label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () { } }, "buscaCep");
                new Button({ label: "", iconClass: 'dijitEditorIcon dijitEditorIconEmail', onClick: function () { enviarEmail("email"); } }, "enviarEmail");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFKRelac();
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) { if (e.keyCode == 13) montarPesquisaPessoaFKPorTipo(); });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        montarPesquisaPessoaFKPorTipo();
                                    });
                                });
                            else
                                abrirPessoaFKRelac();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "buscaPessoaRelac");
                // Criação botões tab Endereços
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemRua", null);
                            clearForm("formCadRua");
                            dojo.byId("setDefaultValueCEP").value = ENDERECOPRINCIPAL;
                            var nome = "dialogRua";
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadRua");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            cadastrarRua();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirRua");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemBairro", null);
                            clearForm("formCadBairro");
                            dojo.byId("setDefaultValueCEP").value = ENDERECOPRINCIPAL;
                            var nome = "dialogBairro";
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadBairro");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemCidade", null);
                            clearForm("formCadCidadeDialog");
                            dojo.byId("setDefaultValueCEP").value = ENDERECOPRINCIPAL;
                            var nome = "dialogCidade";
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadCidade");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            cadastrarCidade();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirCidade");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            cadastrarBairro();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirBairro");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaCidade")))
                                montargridPesquisaCidade(function () {
                                    abrirPesquisaCidadeFKPessoaRelacionada();
                                    dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                                    dijit.byId("pesquisar").on("click", function (e) {
                                        pesquisaCidadeFK();
                                    });
                                });
                            else
                                abrirPesquisaCidadeFKPessoaRelacionada();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "localNascimento");
                // Criação dos botoes Outros Contatos
                //Botões Dialog Outros Contatos
                new Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () { incluirOutrosContatos(); } }, "incluirOutrosContatos");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () { limparOutrosContatos(); } }, "clearOutrosContatos");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogContato").hide(); } }, "fecharOutrosContatos");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                        try {
                            IncluirAlterar(1, 'divAlterarOutrosContatos', 'divIncluirOutrosContatos', 'divExcluirOutrosContatos', dojo.byId("descApresMsg").value, 'divCancelarOutrosContatos', 'divClearOutrosContatos');
                            apresentaMensagem(dojo.byId("descApresMsg").value, null);
                            limparOutrosContatos();
                            dijit.byId("dialogContato").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnNovoContato");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepVelusOutrosContatos(); } }, "cancelarOutrosContatos");
                new Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { altararOutrosContatos(); } }, "alterarOutrosContatos");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { excluirOutrosContatos(); } }, "deleteOutrosContatos");
                // Criação dos botoes Outros Endereços
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                        try {
                            IncluirAlterar(1, 'divAlterarOutrosEnd', 'divIncluirOutrosEnd', 'divExcluirOutrosEnd', dojo.byId("descApresMsg").value, 'divCancelarOutrosEnd', 'divCleanOutrosEnd');
                            apresentaMensagem(dojo.byId("descApresMsg").value, null);
                            limparOutrosEnderecos();
                            dijit.byId("DialogEndereco").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnNovoEndereco");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemRua", null);
                            clearForm("formCadRua");
                            dojo.byId("setDefaultValueCEP").value = OUTROSENDERECOS;
                            var nome = "dialogRua";
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnRuaOutrosEnd");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemBairro", null);
                            clearForm("formCadBairro");
                            dojo.byId("setDefaultValueCEP").value = OUTROSENDERECOS;
                            var nome = "dialogBairro";
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnBairroOutrosEnd");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemCidade", null);
                            clearForm("formCadCidadeDialog");
                            var nome = "dialogCidade";
                            dojo.byId("setDefaultValueCEP").value = OUTROSENDERECOS;
                            montarDialog(nome, e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnCidadeDialog");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            cadastrarCidade("desCidadeOutrsEnd", "cidadeOutrosEnd", dijit.byId('estadoOutrosEnd').value, "dialogCidadeOutrosEnd", "formCadCidadeOutrosEnd", "apresentadorMsgOutosEnderecos", "apresentadorMensagemCidadeOutrosEnd");
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "IncluirCidadeOutrosEnd");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            cadastrarCidade("desCidadeRelac", "cidadeRelac", dijit.byId('estadoRelac').value, "dialogCidadeRelac", "formCadCidadeRelac", "apresMsgNewRelac", "apresentadorMensagemCidadeRelac");
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "IncluirCidadeRelac");

                new Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () { incluirOutrosEndreços(); } }, "incluirOutrosEnd");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () { limparOutrosEnderecos(); } }, "cleanOutrosEnd");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("DialogEndereco").hide(); } }, "fecharOutrosEnd");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepVelusOutrosEnderecos(); } }, "cancelarOutrosEnd");
                new Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { alterarOutrosEnderecos(); } }, "alterarOutrosEnd");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { excluirOutrosEnd(); } }, "deleteOutrosEnd");
                // Criação dos botoes Pessoas Relacionadas
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function (e) {
                        try {
                            apresentaMensagem("apresentadorMensagemRuaRelac", null);
                            clearForm("formCadRuaRelac");
                            montarDialog("dialogRuaRelac", e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadRuaRelac");
                new Button({ label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () { pesquisarCepEnderecosPessoaRelac(); } }, "buscaCepRelac");
                new Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () { incluirNewRelac(); } }, "incluirPessoaRelac");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () { limparNewRelac(); } }, "cleanPessoaRelac");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("DialogRelac").hide(); } }, "fecharPessoaRelac");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { excluirRelac(); } }, "deletePessoaRelac");

                //#region  Auxiliares de pessoa FK
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function (e) {
                        try {
                            clearForm("formAtividade");
                            var natureza = dijit.byId('naturezaPessoa').get('value');

                            if (parseInt(natureza) == PESSOAFISICA) {
                                dojo.byId('codNaturezaAtividade').value = PESSOAFISICA;
                                dojo.byId('id_natureza_atividade').value = 'Física';
                                dojo.byId("tdCnae").style.display = 'none'
                                dojo.byId("tdCdCnaeAtividade").style.display = 'none'
                            } else {
                                dojo.byId('codNaturezaAtividade').value = PESSOAJURIDICA;
                                dojo.byId('id_natureza_atividade').value = 'Jurídica';
                                dojo.byId("tdCnae").style.display = ''
                                dojo.byId("tdCdCnaeAtividade").style.display = ''
                            }

                            if (!hasValue(dijit.byId("gridAuxiliaresPessoaFK")))
                                montargridPesquisaAuxiliaresPessoa(
                                    function () {
                                        abrirPesquisaAuxiliaresPessoaFK(natureza);
                                    });
                            else
                                abrirPesquisaAuxiliaresPessoaFK(natureza);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "proAtividadePessoaFk");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function (e) {
                        try {
                            clearForm("formAtividade");
                            var natureza = dijit.byId('naturezaPessoa').get('value');

                            if (parseInt(natureza) == PESSOAFISICA) {
                                dojo.byId('codNaturezaAtividade').value = PESSOAFISICA;
                                dojo.byId('id_natureza_atividade').value = 'Física';
                                dojo.byId("tdCnae").style.display = 'none'
                                dojo.byId("tdCdCnaeAtividade").style.display = 'none'
                            } else {
                                dojo.byId('codNaturezaAtividade').value = PESSOAJURIDICA;
                                dojo.byId('id_natureza_atividade').value = 'Jurídica';
                                dojo.byId("tdCnae").style.display = ''
                                dojo.byId("tdCdCnaeAtividade").style.display = ''
                            }

                            montarDialog("formularioAtividade", e);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadAtividade");


                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () {
                        try {
                            var gridAuxiliaresPessoa = hasValue(dijit.byId('gridAuxiliaresPessoaFK')) ? dijit.byId('gridAuxiliaresPessoaFK') : [];
                            var descricao = '';
                            var cd_axiliar_pessoa = 0;

                            if (gridAuxiliaresPessoa._by_idx.length > 0 && gridAuxiliaresPessoa.itensSelecionados.length > 1) {
                                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                                return false;
                            }
                            if (gridAuxiliaresPessoa.itensSelecionados == null || gridAuxiliaresPessoa.itensSelecionados.length == 0) {
                                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                                return false;
                            }
                            if (gridAuxiliaresPessoa._by_idx.length > 0 && gridAuxiliaresPessoa.itensSelecionados.length > 0) {
                                descricao = gridAuxiliaresPessoa.itensSelecionados[0].no_atividade;
                                cd_axiliar_pessoa = gridAuxiliaresPessoa.itensSelecionados[0].cd_atividade;
                                var id_natureza_atividade = gridAuxiliaresPessoa.itensSelecionados[0].id_natureza_atividade;
                                if (id_natureza_atividade == CARGO) {
                                    var isPesquisa = $.parseJSON(dojo.byId('isPesquisa').value);
                                    if (!isPesquisa) {
                                        dojo.byId("cdCargo").value = cd_axiliar_pessoa;
                                        dijit.byId("desCargo").set('value', descricao);
                                        dijit.byId("limparFkCargo").set("disabled", false);
                                    } else {
                                        dojo.byId("pesCdAtividade").value = cd_axiliar_pessoa;
                                        dijit.byId("atividadeFunc").set('value', descricao);
                                        dijit.byId("limparFkCargoPes").set("disabled", false);
                                    }
                                } else {
                                    dojo.byId("cdAtividade").value = cd_axiliar_pessoa;
                                    dijit.byId('atividadePrincipal').set('value', descricao);
                                    dijit.byId('limparAtividade').set("disabled", false);
                                }
                                dijit.byId("proAuxiliaresPessoaFK").hide();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionarAtividadeFK");

                new Button({
                    label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdAtividade').value = 0;
                            dojo.byId("atividadePrincipal").value = "";
                            dijit.byId('limparAtividade').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAtividade");
                if (hasValue(document.getElementById("limparAtividade"))) {
                    document.getElementById("limparAtividade").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAtividade").parentNode.style.width = '40px';
                }
                //#endregion

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        incluirAtividade();
                    }
                }, "incluirAtividade");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridLogradouroFK")))
                                montargridPesquisaLogradouroFK(function () { abrirPesquisaLogradouroFK(); });
                            else
                                abrirPesquisaLogradouroFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesLogradouro");
                var pesLogradouro = document.getElementById('pesLogradouro');
                if (hasValue(pesLogradouro)) {
                    pesLogradouro.parentNode.style.minWidth = '18px';
                    pesLogradouro.parentNode.style.width = '18px';
                }
                //Botão Pesquisa logradouro outros endereços.
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridLogradouroFK")))
                                montargridPesquisaLogradouroFK(function () { abrirPesquisaLogradouroFKOutrosEnd(); });
                            else
                                abrirPesquisaLogradouroFKOutrosEnd();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesLogradouroOutrosEnd");
                var pesLogradouroOutrosEnd = document.getElementById('pesLogradouroOutrosEnd');
                if (hasValue(pesLogradouroOutrosEnd)) {
                    pesLogradouroOutrosEnd.parentNode.style.minWidth = '18px';
                    pesLogradouroOutrosEnd.parentNode.style.width = '18px';
                }

                //Botão limpar prospect
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        try {
                            if (hasValue(limparProspect)) {
                                dijit.byId("limparProspect").set("disabled", true);
                                limparProspect();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparProspect");
                decreaseBtn(document.getElementById("limparProspect"), '40px');

                //decreaseBtn(document.getElementById("pesLogradouro"), '18px');
                //link Relacionamentos
                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { excluirRelac(); }
                });
                menu.addChild(acaoRemover);

                var buttonARL = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasRelac",
                    dropDown: menu,
                    id: "acoesRelacionadasRelac"
                });
                dojo.byId("linkAcoesRelac").appendChild(buttonARL.domNode);
                //criação dorpDown incluir novo relacionamento
                var buttonRelac = new DropDownButton({
                    label: "Incluir",
                    name: "btnNovoRelac",
                    dropDown: menu,
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    id: "btnNovoRelac"
                });
                dojo.byId("btnNovoRelac").appendChild(buttonRelac.domNode);

                //Lista de botões a serem diminuidos.
                var buttonFkArray = ['pesPessoaFK', 'localNascimento', 'btnCepDialog', 'btnBairroOutrosEnd', 'btnRuaOutrosEnd', 'btnPessoaRealcDialog', 'pesquisarPessoa', 'PesquisaCidadeFK', 'buscaCep',
                'cadBairro', 'cadRua', 'buscaPessoaRelac', 'buscaCepRelac', 'cadRuaRelac', 'cadAtividade',
                'cadCidade', 'btnCidadeDialog', 'enviarEmail', 'proAtividadePessoaFk'];
                diminuirBotoes(buttonFkArray);

                if (hasValue(document.getElementById("uploader"))) {
                    document.getElementById("uploader").parentNode.style.minWidth = '30px';
                    document.getElementById("uploader").parentNode.style.width = '30px';
                }
                var excluirFoto = document.getElementById("excluirFoto");
                if (hasValue(excluirFoto)) {
                    excluirFoto.parentNode.style.minWidth = '35px';
                    excluirFoto.parentNode.style.width = '35px';
                    if (hasValue(dijit.byId('excluirFoto')))
                        dijit.byId('excluirFoto').set('disabled', true);
                }
                if (!hasValue(dijit.byId("uploader_label"))) {
                    $("#uploader_label").css("padding", "0px");
                }
                //Funcao que altera as tabs Pessoa Fisica e Pessoa Juridica
                dijit.byId("naturezaPessoa").on("change", function (e) {
                    try {
                        if (hasValue(e)) {
                            natureza = e;
                        }
                        if (e == 1) {
                            dojo.byId('panelPessoaJuridica').style.display = 'none';
                            dojo.byId('panelPessoaFisica').style.display = 'block';
                            document.getElementById('trCPF').style.display = 'block';
                            document.getElementById('trCNPJ').style.display = 'none';
                            document.getElementById('cpf_cnpj').innerHTML = 'CPF:';
                            dijit.byId("sexo").domNode.style.display = '';
                            document.getElementById('lbSexo').style.display = '';
                            document.getElementById('trSexo').style.display = '';
                        }
                        if (e == 2) {
                            dojo.byId('panelPessoaFisica').style.display = 'none';
                            dojo.byId('panelPessoaJuridica').style.display = 'block';
                            document.getElementById('trCPF').style.display = 'none';
                            document.getElementById('trCNPJ').style.display = 'block';
                            document.getElementById('cpf_cnpj').innerHTML = 'CNPJ:';
                            dijit.byId("sexo").domNode.style.display = 'none';
                            document.getElementById('lbSexo').style.display = 'none';
                            document.getElementById('trSexo').style.display = 'none';
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("naturezaPessoaRelac").on("change", function (e) {
                    try {
                        if (e == PESSOAFISICA) {
                            dojo.byId("laCPfCnpj").innerHTML = "CPF:";
                            $("#inputRelac").mask("999.999.999-99");
                            dojo.byId('tdSexo').style.display = '';
                            dojo.byId('tdSexo2').style.display = '';
                            dijit.byId("grauParent").set("required", true);
                        }
                        if (e == PESSOAJURIDICA) {
                            dojo.byId("laCPfCnpj").innerHTML = "CNPJ:";
                            $("#inputRelac").mask("99.999.999/9999-99");
                            dojo.byId('tdSexo').style.display = 'none';
                            dojo.byId('tdSexo2').style.display = 'none';
                            dijit.byId("grauParent").set("required", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("sexo").on("change", function (e) {
                    try {
                        loadValidSexo(e);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("naturezaPessoa").on("change", function (e) {
                    try {
                        if (e == PESSOAFISICA)
                            dojo.byId("labelAtividade").innerHTML = "Profissão:";
                        else
                            dojo.byId("labelAtividade").innerHTML = "Atividade Principal:";
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                //Change dos campos do endereço principal.
                //registry.byId("estado").on("click", function (e) {
                //    clearCidade();
                //});
                registry.byId("estado").on("change", function (e) {
                    try {
                        var compCidade = dijit.byId("cidade");
                        if (!dojo.byId("estado").readOnly)
                            if (e != null && e > 0) {
                                if (e != registry.byId("estado").oldValue) {
                                    registry.byId("estado").oldValue = e;
                                    compCidade.set("readOnly", false)
                                    //dijit.byId("cadCidade").set("disabled", false);
                                    compCidade._onChangeActive = false;
                                    compCidade.reset();
                                    compCidade._onChangeActive = true;
                                    carregarCidadePorEstado(e, "cidade", 'apresentadorMensagemPessoa');
                                }
                            } else {
                                registry.byId("estado").oldValue = 0;
                                compCidade.set("readOnly", true)
                                compCidade.reset();
                                //dijit.byId("cadCidade").set("disabled", true);
                            }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("cidade").on("change", function (e) {
                    try {
                        var compBairro = dijit.byId("bairro");
                        if (!dojo.byId("cidade").readOnly)
                            if (e != null && e > 0) {
                                compBairro.set("readOnly", false)
                                //dijit.byId("cadBairro").set("disabled", false);
                                compBairro._onChangeActive = false;
                                compBairro.reset();
                                compBairro._onChangeActive = true;
                                carregarBairroPorCidade(e, "bairro", 'apresentadorMensagemPessoa');
                            } else {
                                compBairro.set("readOnly", true)
                                //dijit.byId("cadBairro").set("disabled", true);
                                compBairro.reset();
                            }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("bairro").on("change", function (e) {
                    try {
                        var compRua = dijit.byId("rua");
                        if (e != dijit.byId("bairro").oldValue)
                            if (!dojo.byId("cidade").readOnly)
                                if (e != null && e > 0) {
                                    dojo.byId("codRua").value = 0;
                                    compRua._onChangeActive = false;
                                    compRua.reset();
                                    compRua._onChangeActive = true;
                                    compRua.reset();
                                }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                //Change dos campos do outros endereços.
                registry.byId("estadoOutrosEnd").on("change", function (e) {
                    try {
                        var compCidadeOutrosEnd = dijit.byId("cidadeOutrosEnd");
                        if (!dojo.byId("estadoOutrosEnd").readOnly)
                            if (e != null && e > 0) {
                                if (e != registry.byId("estadoOutrosEnd").oldValue) {
                                    registry.byId("estadoOutrosEnd").oldValue = e;
                                    compCidadeOutrosEnd.set("readOnly", false)
                                    //dijit.byId("btnCidadeDialog").set("disabled", false);
                                    compCidadeOutrosEnd._onChangeActive = false;
                                    compCidadeOutrosEnd.reset();
                                    compCidadeOutrosEnd._onChangeActive = true;
                                    carregarCidadePorEstado(e, "cidadeOutrosEnd", 'apresentadorMsgOutosEnderecos');
                                }
                            } else {
                                compCidadeOutrosEnd.set("readOnly", true)
                                compCidadeOutrosEnd.reset();
                                //dijit.byId("btnCidadeDialog").set("disabled", true);
                            }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("cidadeOutrosEnd").on("change", function (e) {
                    try {
                        var compBairroOutrosEnd = dijit.byId("bairroOutrosEnd");
                        if (!dojo.byId("cidadeOutrosEnd").readOnly)
                            if (e != null && e > 0) {
                                compBairroOutrosEnd.set("readOnly", false)
                                //dijit.byId("btnBairroOutrosEnd").set("disabled", false);
                                compBairroOutrosEnd._onChangeActive = false;
                                compBairroOutrosEnd.reset();
                                compBairroOutrosEnd._onChangeActive = true;
                                carregarBairroPorCidade(e, "bairroOutrosEnd", 'apresentadorMensagemPessoa');
                            } else {
                                compBairroOutrosEnd.set("readOnly", true)
                                //dijit.byId("btnBairroOutrosEnd").set("disabled", true);
                                compBairroOutrosEnd.reset();
                            }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                 registry.byId("bairroOutrosEnd").on("change", function (e) {
                    try {
                        var compRuaOutrosEnd = dijit.byId("ruaOutrosEnd");
                        if (e != dijit.byId("bairroOutrosEnd").oldValue)
                            if (!dojo.byId("cidadeOutrosEnd").readOnly)
                                if (e != null && e > 0) {
                                    dojo.byId("codRuaOutrosEnd").value = 0;
                                    compRuaOutrosEnd._onChangeActive = false;
                                    compRuaOutrosEnd.reset();
                                    compRuaOutrosEnd._onChangeActive = true;
                                    compRuaOutrosEnd.reset();
                                }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                registry.byId("proPessoa").on("Show", function (e) {
                    try {
                        dijit.byId("gridPesquisaPessoa")._clearData();
                        dijit.byId("gridPesquisaPessoa")._by_idx = [];
                        dijit.byId("gridPesquisaPessoa").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("tipoContatoDialog").on("change", function (e) {
                    try {
                        if (e == CELULAR || e == TELEFONE) {
                            if (e == CELULAR)
                                $("#telefone").mask("(99) 9999-9999");
                            else
                                $("#descTelefoneDialog").mask("(99) 9999-9999");
                        } else
                            $("#descTelefoneDialog").unmask();

                        if (e == CELULAR)
                            document.getElementById('trOperadoraDialog').style.display = '';
                        else
                            document.getElementById('trOperadoraDialog').style.display = 'none';
                        //if (e == EMAIL) {
                        //}
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("tagComplemento").on("Show", function (e) {
                    try {
                        dijit.byId("gridEnderecos").update();
                        dijit.byId("gridContatos").update();
                        dijit.byId("gridRelacionamentos").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("panelOutrosEnderecos").on("Show", function (e) {
                    try {
                        dijit.byId("gridEnderecos").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("panelOutrosContatos").on("Show", function (e) {
                    try {
                        dijit.byId("gridContatos").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("panelPessoaRelacionadas").on("Show", function (e) {
                    try {
                        dijit.byId("gridRelacionamentos").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("excluirFoto").on("click", function (e) {
                    try {
                        dojo.byId("setValueExt_foto_Pessoa").value = "";
                        dijit.byId('excluirFoto').setAttribute('disabled', 1);
                        $("#foto").attr({
                            src: "",
                            title: ""
                        }).css("display", "none");
                        if (hasValue(dijit.byId("uploader")))
                            dijit.byId("uploader")._files = [];
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("cep").on("change", function (e) {
                    try {
                        if (hasValue(e) && e != "_____-___")
                            pesquisarCepPessoa(e, ENDERECOPRINCIPAL, dojo.byId("descApresMsg").value);
                        else
                            configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cepOutrosEnd").on("change", function (e) {
                    try {
                        if (hasValue(e) && e != "_____-___")
                            pesquisarCepPessoa(e, OUTROSENDERECOS, "apresentadorMsgOutosEnderecos");
                        else {
                            configurarLayoutEndereco(LAYOUTPADRAO, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd", "ruaOutrosEnd",
                                                                   "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cepRelac").on("change", function (e) {
                    if (hasValue(dojo.byId("cepRelac").value))
                        pesquisarCepEnderecosPessoaRelac();
                })
                registry.byId("limparEndereoPrincipal").on("click", function (e) {
                    try {
                        limparEnderecoPrincipal();
                        configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("pesPessoaFK").on("click", function (e) {
                    try {
                        dojo.byId("setValuePesquisaPessoaFK").value = USARCPF;
                        if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                            montargridPesquisaPessoa(function () {
                                abrirPessoaFKCPF();
                                dojo.query("#_nomePessoaFK").on("keyup", function (e) { if (e.keyCode == 13) montarPesquisaPessoaFKPorTipo(); });
                                dijit.byId("pesqPessoa").on("click", function (e) {
                                    montarPesquisaPessoaFKPorTipo();
                                });

                            });
                        else
                            abrirPessoaFKCPF();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("btnCidadeDialog").set("disabled", true);
                dijit.byId("btnCidadeDialog").set("disabled", true);
                dijit.byId("inputRelac").on("blur", function (evt) {
                    try {
                        if (hasValue(dojo.byId("nmNaturezaPapel").value) && (dojo.byId("nmNaturezaPapel").value == PAPELCPF
                                || (dojo.byId("nmNaturezaPapel").value == PAPELCPFCNPJ && hasValue(dijit.byId('naturezaPessoaRelac')) && dijit.byId('naturezaPessoaRelac').value == PAPELCPF))) {
                            if (trim(dojo.byId("inputRelac").value) != "" && dojo.byId("inputRelac").value != "___.___.___-__")
                                if (validarCPF("#inputRelac", "apresMsgNewRelac"))
                                    //validarCPF() ?
                                    verificarPessoCPF();
                        }
                        else {
                            if (trim(dojo.byId("inputRelac").value) != "" && dojo.byId("inputRelac").value != "__.___.___/____-__")
                                if (validarCnpj("#inputRelac", "apresMsgNewRelac"))
                                    //validarCPF() ?
                                    verificarEmpresaByCnpj("#inputRelac", "apresMsgNewRelac");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId('uploader').on("change", function (evt) {
                    try {
                        var mensagensWeb = new Array();
                        var files = dijit.byId("uploader")._files;
                        apresentaMensagem(dojo.byId("descApresMsg").value, null);
                        if (hasValue(files) && files.length > 0) {
                            if (hasValue(files[0]) && files[0].size > 400000) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanho);
                                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                                return false;
                            }
                            if (!verificarExtensaoArquivo(files[0].name)) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtesaoErradaArquivo);
                                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                                return false;
                            }
                            if (window.FormData !== undefined) {
                                var data = new FormData();
                                data.append("UploadedImage", files[0]);
                                $.ajax({
                                    type: "POST",
                                    url: Endereco() + "/api/secretaria/UploadImage",
                                    ansy: false,
                                    headers: { Authorization: Token() },
                                    contentType: false,
                                    processData: false,
                                    data: data,
                                    success: function (results) {
                                        results = jQuery.parseJSON(results);
                                        loadFoto(results);
                                        dijit.byId('excluirFoto').setAttribute('disabled', 0);
                                    },
                                    error: function (error) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Impossível fazer upload de foto. Verifique se o tamanho dela é abaixo de 1 MB.");
                                        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                                    }
                                });
                            } else {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de foto. Verifique se o tamanho dela é abaixo de 1 MB.");
                                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId('pesEndResp').on("click", function (evt) {
                    pesquisarEnderecoResponsavel();
                });               
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
};

function pesquisarEnderecoResponsavel() {
    var cd_pessoa_responsavel_CPF = dojo.byId("cdPessoaCpf").value;
    if (!hasValue(cd_pessoa_responsavel_CPF) || cd_pessoa_responsavel_CPF == "0" && hasPessoaRelacionadaResponsavel() == true) {
        var pessoaFilter = getPessoaRelacionadaResponsavel();
        if (pessoaFilter != null && pessoaFilter != undefined && pessoaFilter.length > 0) {
            cd_pessoa_responsavel_CPF = pessoaFilter[0].relacionamento.cd_pessoa_filho;
        }
    }
    
    dojo.xhr.get({
        url: Endereco() + "/api/localidade/getEnderecoResponsavelCPF?cd_pessoa=" + cd_pessoa_responsavel_CPF,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem(dojo.byId("descApresMsg").value, data);
            if (hasValue(data.retorno)) {
                data.retorno.cd_endereco = null;
                if (hasValue(dijit.byId("tipoEndereco").get("value")) || hasValue(dijit.byId("logradouro").get("value")) || hasValue(dijit.byId("estado").get("value")) ||
                                 hasValue(dijit.byId("cidade").get("value")) || hasValue(dojo.byId("cep").value) || hasValue(dojo.byId("bairro").value) ||
                                 hasValue(dojo.byId("rua").value) || hasValue(dojo.byId("numero").value) || hasValue(dojo.byId("complemento").value)) {
                    caixaDialogo(DIALOGO_CONFIRMAR, 'Deseja substituir o endereço atual?', function executaRetorno() {
                        data.retorno.cd_endereco = null;
                        setValueEnderecoPrincipal(data.retorno);
                    });
                }
                else
                    setValueEnderecoPrincipal(data.retorno);
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgInfoNaoExisteEndRespo);
                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem(dojo.byId("descApresMsg").value, error);
    });
}

function getPessoaRelacionadaResponsavel() {
    try {
        var pessoa_relacionada_responsavel = [];

        if (hasValue(dijit.byId("gridRelacionamentos")) &&
            dijit.byId("gridRelacionamentos")['store'] != null &&
            dijit.byId("gridRelacionamentos")['store'] != undefined &&
            dijit.byId("gridRelacionamentos")['store']['objectStore'] != null &&
            dijit.byId("gridRelacionamentos")['store']['objectStore'] != undefined &&
            dijit.byId("gridRelacionamentos")['store']['objectStore']['data'] != null &&
            dijit.byId("gridRelacionamentos")['store']['objectStore']['data'] != undefined

        ) {
            if (dijit.byId("gridRelacionamentos").store.objectStore.data.length > 0) {
                pessoa_relacionada_responsavel = dijit.byId("gridRelacionamentos").store.objectStore.data.filter(function (value) {
                    return value != null &&
                        value != undefined &&
                        value['relacionamento'] != null &&
                        value['relacionamento'] != undefined &&
                        (value['relacionamento'].cd_papel_filho == EnumPapelRelacionamento.RESPONSAVEL_FINANCEIRO ||
                            (value['relacionamento'].cd_papel_filho == EnumPapelRelacionamento.RESPONSAVEL_FINANCEIRO + "")) &&
                        (value['relacionamento'].cd_papel_pai == EnumPapelRelacionamento.ALUNO_RESPONSAVEL ||
                            (value['relacionamento'].cd_papel_pai == EnumPapelRelacionamento.ALUNO_RESPONSAVEL + "") ||
                            (value['relacionamento'].cd_papel_pai == "0")) &&
                        value.no_papel == "Responsavel Financeiro";

                });
            }
        }
        return pessoa_relacionada_responsavel;
    } catch (e) {
        postGerarLog(e);
    }
}

function verificarExtensaoArquivo(nomeArquivo) {
    var valido = true;
    var achou = false;
    var TamanhoString = nomeArquivo.length;
    var extensao = nomeArquivo.substr(TamanhoString - 4, TamanhoString);
    var ext = new Array('.JPG', 'JPEG', '.GIF', '.PNG', '.BMP');
    for (var i = 0; i < ext.length; i++)
        if (extensao.toUpperCase() == ext[i]) {
            achou = true;
            break;
        }

    if (!achou)
        valido = false;
    return valido;
}

function updateOutrasTags() {
    try {
        dijit.byId("gridEnderecos").update();
        dijit.byId("gridContatos").update();
        dijit.byId("gridRelacionamentos").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadValidSexo(sexo) {
    try {
        
        if (sexo == MASCULINO) {
	        loadDataPessoaBySexo(MASCULINO, null, null, null);
	        dijit.byId('tratamento').set("disabled", false);
	        dijit.byId('estadoCivil').set("disabled", false);
	        dijit.byId('nacionalidade').set("disabled", false);
	        limparFilhosBySexo();
        } else {
            loadDataPessoaBySexo(FEMININO, null, null, null);
	        dijit.byId('tratamento').set("disabled", false);
	        dijit.byId('estadoCivil').set("disabled", false);
	        dijit.byId('nacionalidade').set("disabled", false);
	        limparFilhosBySexo();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFilhosBySexo() {
    try {
        dijit.byId("tratamento").reset();
        dijit.byId("estadoCivil").reset();
        dijit.byId("nacionalidade").reset();
        dojo.byId("codLocalNascimento").value = "";
        dojo.byId("descLocalNascimento").value = "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPessoaFisica() {
    try {
        clearForm("formSexo");
        dojo.byId("cpf").oldValue = 0;
        dojo.byId("cd_pessoa").value = 0;
        dojo.byId("cdPessoaCpf").value = 0;
        dojo.byId("setDefaultValueCEP").value = 0;
        dojo.byId("setValueExt_foto_Pessoa").value = "";
        clearForm("formCpf");
        clearForm("formCnpj");
        clearForm("formDtaCadastramento");
        clearForm("formNomPessoa");
        clearForm("formUsarCpf");
        dojo.byId("nomFantasia").value = "";
        clearForm("formPessoaFisica");
        dijit.byId("naturezaPessoa").set("value", dijit.byId("naturezaPessoa").value);
        clearForm("FormDescricao");
        dojo.byId("atividadePrincipal").value = "";
        dojo.byId("cdAtividade").value = 0;
        dijit.byId("panelPessoaFisica").set("open", false);
        dijit.byId("pesEndResp").set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPessoaJuridica() {
    try {
        dojo.byId("cd_pessoa").value = 0;
        dojo.byId("setDefaultValueCEP").value = 0;
        dojo.byId("setValuePesquisaPessoaFK").value = 1;
        clearForm("formCpf");
        clearForm("formCnpj");
        clearForm("formDtaCadastramento");
        clearForm("formNomPessoa");
        dojo.byId("nomFantasia").value = "";
        dijit.byId("naturezaPessoa").set("value", dijit.byId("naturezaPessoa").value);
        clearForm("formPessoaJuridica");
        clearForm("FormDescricao");
        dojo.byId("atividadePrincipal").value = "";
        dojo.byId("cdAtividade").value = 0;
        dijit.byId("panelPessoaJuridica").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparEnderecoPrincipal() {
    try {
        dojo.byId("cdEndereco").value = 0;
        //dojo.byId("codBairro").value = 0;
        dojo.byId("codRua").value = 0;
        dijit.byId("operadora").reset();
        dijit.byId("logradouro").reset();
        dijit.byId("tipoEndereco").reset();
        dijit.byId("estado").reset();
        dijit.byId("estado").oldValue = 0;
        dijit.byId("cidade").reset();
        dijit.byId("bairro").reset();
        dijit.byId("bairro").oldValue
        dijit.byId("rua").reset();
        dojo.byId("numero").value = "";
        dojo.byId("complemento").value = "";

        dijit.byId("cadCidade").set("disabled", true);
        dijit.byId("cadBairro").set("disabled", true);
        dijit.byId("cadRua").set("disabled", true);
        habilitaOperadora(dojo.byId("operadora"));
        dojo.byId("cep").value = "";
        dijit.byId("cep").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparContatosPricipais() {
    try {
        dijit.byId("telefone").reset();
        dijit.byId("email").reset();
        dijit.byId("celular").reset();
        dijit.byId("site").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Testa o Email
function isEmail(email) {
    try {
        var check = /@[\w\-]+\./;
        var checkend = /\.[a-zA-Z]{2,5}$/;
        var obj = eval("document.forms[7].email");
        if (hasValue(trim(email))) {
            if (((email.search(check)) == -1) || (email.search(checkend) == -1)) {
                caixaDialogo(MENSAGEM_AVISO, 'E-mail no formato inválido.', "");
                return false;
                obj.focus();
            }
            else { return true; }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function isEmailOutrosContatos(email, id) {
    try {
        var check = /@[\w\-]+\./;
        var checkend = /\.[a-zA-Z]{2,5}$/;
        var obj = dojo.byId("" + id + "");
        var compTipoTelefone = dijit.byId("tipoContatoDialog");
        if (hasValue(compTipoTelefone) && hasValue(compTipoTelefone.value) && compTipoTelefone.value == EMAIL && hasValue(trim(email))) {
            if (((email.search(check)) == -1) || (email.search(checkend) == -1)) {
                caixaDialogo(MENSAGEM_AVISO, 'E-mail no formato inválido.', "");
                return false;
                obj.focus();
            }
            else { return true; }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDialog(nome, e) {
    try {
        var offSetX = 20;
        var offSetY = 20;
        $("#" + nome).css("top", e.pageX + offSetX).css("left", e.pageY + offSetY);
        dijit.byId(nome).show();
        dojo.byId(nome).style.display = "block";
    }
    catch (e) {
        postGerarLog(e);
    }
}

//função que verifica se existe uma pessoa na base com esse cpf e o retorna.
function verificarPessoCPFAllDados() {
    try {
        var mensagensWeb = new Array();
        if ($("#cpf").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/aluno/VerificarExistPessoByCpf?cpfCgc=" + $("#cpf").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    dataPessoaFisica = jQuery.parseJSON(data);
                    apresentaMensagem(dojo.byId("descApresMsg").value, null);
                    if (data.retorno != null) {
                        setarValuePessoaFisica(data.retorno, false);
                    } else {
                        $("#cd_pessoa").val(0);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "CPF válido.");
                        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(dojo.byId("descApresMsg").value, error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarPessoCPF() {
    try {
        var mensagensWeb = new Array();
        if ($("#inputRelac").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/pessoa/VerificarPessoaByCpf?cpf=" + $("#inputRelac").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    apresentaMensagem("apresMsgNewRelac", null);
                    if (data.retorno != null) {
                        dojo.byId("noPessoaRelac").value = data.retorno.no_pessoa;
                        dojo.byId("codPessoaRelac").value = data.retorno.cd_pessoa;
                        dijit.byId("sexoRelac").set("value", data.retorno.nm_sexo);
                        dijit.byId("sexoRelac").set("disabled", true);
                        dijit.byId("inputRelac").set("disabled", true);
                        dijit.byId("noPessoaRelac").set("disabled", true);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                //mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não foi possivel validar CPF.");
                apresentaMensagem("apresMsgNewRelac", error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarValuePessoaJuridica(allDataPessoaJuridica, carregando) {
    try {
        var pessoaJuridica = allDataPessoaJuridica.pessoaJuridica;
        var allTelefones = allDataPessoaJuridica.telefoneUI
        var allOutrosContatos = [];
        var allContatosPrincipais = [];
        var allEnderecos = allDataPessoaJuridica.pessoaJuridica.Enderecos;
        if (hasValue(allDataPessoaJuridica) && hasValue(allDataPessoaJuridica.contatosUI) && allDataPessoaJuridica.contatosUI.outrosContatos.length > 0)
            allOutrosContatos = allDataPessoaJuridica.contatosUI.outrosContatos;
        if (hasValue(allDataPessoaJuridica) && hasValue(allDataPessoaJuridica.contatosUI) && allDataPessoaJuridica.contatosUI.contatosPrincipais.length > 0)
            allContatosPrincipais = allDataPessoaJuridica.contatosUI.contatosPrincipais;
        var allRelacionamentos = allDataPessoaJuridica.relacionamentoUI;
        //Dados Pessoa
        if (pessoaJuridica.cd_pessoa > 0)
            dijit.byId("naturezaPessoa").set("value", pessoaJuridica.nm_natureza_pessoa);
        dojo.byId("setValueExt_foto_Pessoa").value = hasValue(pessoaJuridica.ext_img_pessoa) ? pessoaJuridica.ext_img_pessoa : "";
        hasValue(pessoaJuridica.cd_pessoa) ? dojo.byId('cd_pessoa').value = pessoaJuridica.cd_pessoa : 0;
        dojo.byId("cnpj").value = pessoaJuridica.dc_num_cgc;
        pessoaJuridica.no_pessoa != null ? dojo.byId("nomPessoa").value = pessoaJuridica.no_pessoa : "";
        pessoaJuridica.dc_reduzido_pessoa != null ? dojo.byId("nomFantasia").value = pessoaJuridica.dc_reduzido_pessoa : "";
        pessoaJuridica.dta_cadastro != null ? dojo.byId("dtaCadastro").value = pessoaJuridica.dta_cadastro : "";
        pessoaJuridica.hr_cadastro != null ? dojo.byId("horaCadastro").value = pessoaJuridica.hr_cadastro : "";

        //Dados Pessoa Juridica
        dijit.byId("tipoSociedade").set("value", pessoaJuridica.cd_tipo_sociedade);
        dojo.byId("dcNumInscEstadual").value = pessoaJuridica.dc_num_insc_estadual;
        dojo.byId("dcNumInscMunicipal").value = pessoaJuridica.dc_num_insc_municipal;
        dojo.byId("dcNumCnpjCnab").value = pessoaJuridica.dc_num_cnpj_cnab;
        dojo.byId("dtRegistroJuntaComercial").value = pessoaJuridica.dtaRJuntaComer;
        dojo.byId("dtaBaixa").value = pessoaJuridica.dtaBaixa;

        if (hasValue(pessoaJuridica.EnderecoPrincipal))
            setValueEnderecoPrincipal(pessoaJuridica.EnderecoPrincipal);
        if (hasValue(allContatosPrincipais))
            setValueContatosPrincipais(allContatosPrincipais, pessoaJuridica.cd_telefone_principal);
        if (hasValue(allOutrosContatos))
            setarValueOutrosContatos(allOutrosContatos);
        if (hasValue(allEnderecos && allEnderecos.length > 0))
            setValueAllEnderecos(allEnderecos);
        if (hasValue(allRelacionamentos))
            setvalueRelacionamentos(allRelacionamentos);
        if (pessoaJuridica.Atividade != null) {
            dojo.byId("atividadePrincipal").value = hasValue(pessoaJuridica.Atividade.no_atividade) ? pessoaJuridica.Atividade.no_atividade : "";
            dojo.byId("cdAtividade").value = hasValue(pessoaJuridica.Atividade.cd_atividade) ? pessoaJuridica.Atividade.cd_atividade : 0;
            dijit.byId('limparAtividade').set("disabled", false);
        }
        (hasValue(pessoaJuridica.txt_obs_pessoa)) ? dijit.byId('descObs').set("value", pessoaJuridica.txt_obs_pessoa) : dijit.byId('descObs').set("value", "");
        if (hasValue(carregando) && carregando)
            showCarregando();
        observeTitlePaneOpenUpdate();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadAlunoRaf(PessoaRaf) {
    try {
        if (hasValue(PessoaRaf) && PessoaRaf != null && PessoaRaf.length > 0) {
            alterarVisibilidadeTabAlunoRaf(true);

            if (dojo.byId("cd_pessoa_raf") != null && dojo.byId("cd_pessoa_raf") != undefined) {
                dojo.byId("cd_pessoa_raf").value = PessoaRaf[0].cd_pessoa_raf;
            }

            if (dojo.byId("raf_cd_pessoa") != null && dojo.byId("raf_cd_pessoa") != undefined) {
                dojo.byId("raf_cd_pessoa").value = PessoaRaf[0].cd_pessoa;
            }

            if (hasValue(dijit.byId("nmRaf"))) {
                dijit.byId("nmRaf").set("value", PessoaRaf[0].nm_raf);
                dijit.byId("nmRaf").set("disabled", true);
            }

            if (hasValue(dijit.byId("ckLiberado"))) {
                dijit.byId("ckLiberado").set("value", PessoaRaf[0].id_raf_liberado);
                if (eval(MasterGeral()) == false) {
                    dijit.byId("ckLiberado").set("disabled", true);
                } else {
                    dijit.byId("ckLiberado").set("disabled", false);
                }
                
            }

            if (hasValue(dijit.byId("ckBloqueado"))) {
                dijit.byId("ckBloqueado").set("value", PessoaRaf[0].id_bloqueado);
                dijit.byId("ckBloqueado").set("disabled", true);
            }

            if (hasValue(dijit.byId("ckTrocarSenha"))) {
                dijit.byId("ckTrocarSenha").set("value", PessoaRaf[0].id_trocar_senha);
                dijit.byId("ckTrocarSenha").set("disabled", true);
            }

            if (dijit.byId("nmTentativas") != null && dijit.byId("nmTentativas") != undefined) {
                dijit.byId("nmTentativas").set("value", PessoaRaf[0].nm_tentativa);
                dijit.byId("nmTentativas").set("disabled", true);
            }

            if (hasValue(dojo.byId("dt_expiracao_senha"))) {
                dijit.byId("dt_expiracao_senha")._onChangeActive = false;
                dojo.byId("dt_expiracao_senha").value = PessoaRaf[0].dta_expiracao_senha;
                dijit.byId("dt_expiracao_senha")._onChangeActive = true;
                dijit.byId("dt_expiracao_senha").set("disabled", true);
            }

            if (hasValue(dojo.byId("dt_limite_bloqueio"))) {
                dijit.byId("dt_limite_bloqueio")._onChangeActive = false;
                dojo.byId("dt_limite_bloqueio").value = PessoaRaf[0].dta_limite_bloqueio;
                dijit.byId("dt_limite_bloqueio")._onChangeActive = true;
                if (eval(MasterGeral()) == false) {
                    dijit.byId("dt_limite_bloqueio").set("disabled", true);
                } else {
                    dijit.byId("dt_limite_bloqueio").set("disabled", false);
                }
            }

            if (hasValue(dijit.byId("nmRaf"))) {
                if (PessoaRaf[0].hasPassCreated == true) {
                    dijit.byId("enviarEmailRafAluno").set("disabled", true);
                } else {
                    dijit.byId("enviarEmailRafAluno").set("disabled", false);
                }
                
            }




        } else {
            if (typeof alterarVisibilidadeTabAlunoRaf === "function") {
                alterarVisibilidadeTabAlunoRaf(false);
            }
            
        }
        
    } catch (e) {
        postGerarLog(e);
    } 
}

function setarValuePessoaFisica(allDataPessoaFisica, carregando) {
    try {
        var pessoaFisica = allDataPessoaFisica.pessoaFisica;
        if (hasValue(pessoaFisica)) {
            dojo.byId("descObs").value = hasValue(pessoaFisica.txt_obs_pessoa) ? pessoaFisica.txt_obs_pessoa : "";
            var allEnderecos = allDataPessoaFisica.pessoaFisica.Enderecos;
            var allRelacionamentos = allDataPessoaFisica.relacionamentoUI;
            var allOutrosContatos = [];
            var allContatosPrincipais = [];
            if (hasValue(allDataPessoaFisica) && hasValue(allDataPessoaFisica.contatosUI) && allDataPessoaFisica.contatosUI.outrosContatos.length > 0)
                allOutrosContatos = allDataPessoaFisica.contatosUI.outrosContatos;
            if (hasValue(allDataPessoaFisica) && hasValue(allDataPessoaFisica.contatosUI) && allDataPessoaFisica.contatosUI.contatosPrincipais.length > 0)
                allContatosPrincipais = allDataPessoaFisica.contatosUI.contatosPrincipais;
            var allRelacionamentos = allDataPessoaFisica.relacionamentoUI;
            var sexo = dijit.byId("sexo");
            if (pessoaFisica.cd_pessoa > 0)
                dijit.byId("naturezaPessoa").set("value", pessoaFisica.nm_natureza_pessoa)
            dojo.byId("cpf").value = hasValue(pessoaFisica.nm_cpf) ? pessoaFisica.nm_cpf : "";
            dojo.byId("cpf").oldValue = hasValue(pessoaFisica.nm_cpf) ? pessoaFisica.nm_cpf : "";
            hasValue(pessoaFisica.cd_pessoa) ? dojo.byId('cd_pessoa').value = pessoaFisica.cd_pessoa : 0;
            if (pessoaFisica.nm_sexo > 0) {
                sexo._onChangeActive = false;
                pessoaFisica.nm_sexo != null ? sexo.set('value', pessoaFisica.nm_sexo) : "";
                sexo._onChangeActive = true;
                loadValidSexo(pessoaFisica.nm_sexo);
                loadDataPessoaBySexo(pessoaFisica.nm_sexo, pessoaFisica.cd_estado_civil, pessoaFisica.cd_tratamento, pessoaFisica.cd_loc_nacionalidade);
            }
            if (hasValue(pessoaFisica.cd_pessoa_cpf) && pessoaFisica.cd_pessoa_cpf != 0) {
                //dijit.byId("pesEndResp").set("disabled", false);
                dojo.byId("cdPessoaCpf").value = pessoaFisica.cd_pessoa_cpf;
                dojo.byId("nomPessoaCpf").value = hasValue(allDataPessoaFisica.no_pessoa_cpf) ? allDataPessoaFisica.no_pessoa_cpf : "";
            } /*else {
                //dijit.byId("pesEndResp").set("disabled", true);
            }*/

            dojo.byId("setValueExt_foto_Pessoa").value = hasValue(pessoaFisica.ext_img_pessoa) ? pessoaFisica.ext_img_pessoa : "";
            pessoaFisica.no_pessoa != null ? dojo.byId("nomPessoa").value = pessoaFisica.no_pessoa : "";
            pessoaFisica.dc_reduzido_pessoa != null ? dojo.byId("nomFantasia").value = pessoaFisica.dc_reduzido_pessoa : "";
            dojo.byId("atividadePrincipal").value = hasValue(pessoaFisica.Atividade) ? pessoaFisica.Atividade.no_atividade : "";
            pessoaFisica.dta_cadastro != null ? dojo.byId("dtaCadastro").value = pessoaFisica.dta_cadastro : "";
            pessoaFisica.hr_cadastro != null ? dojo.byId("horaCadastro").value = pessoaFisica.hr_cadastro : "";
            pessoaFisica.cd_estado_civil != null ? dijit.byId("estadoCivil").set("value", pessoaFisica.cd_estado_civil) : 0;
            pessoaFisica.cd_loc_nacionalidade != null ? dijit.byId("nacionalidade").set("value", pessoaFisica.cd_loc_nacionalidade) : 0;
            pessoaFisica.nm_doc_identidade != null ? dijit.byId("nroRg").set("value", pessoaFisica.nm_doc_identidade) : "";
            pessoaFisica.cd_orgao_expedidor != null ? dijit.byId("orgExp").set("value", pessoaFisica.cd_orgao_expedidor) : 0;
            pessoaFisica.cd_estado_expedidor != null ? dijit.byId("expRG").set("value", pessoaFisica.cd_estado_expedidor) : 0;
            pessoaFisica.dtaEmisExpedidor != null ? dojo.byId("dtaEmiRg").value = pessoaFisica.dtaEmisExpedidor : "";
            pessoaFisica.cd_tratamento != null ? dijit.byId("tratamento").set("value", pessoaFisica.cd_tratamento) : 0;
            pessoaFisica.dc_num_insc_inss != null ? dojo.byId("nroInss").value = pessoaFisica.dc_num_insc_inss : "";
            pessoaFisica.dc_carteira_trabalho != null ? dojo.byId("carteiraTrabalho").value = pessoaFisica.dc_carteira_trabalho : "";
            pessoaFisica.dc_carteira_motorista != null ? dojo.byId("carteiraMotorista").value = pessoaFisica.dc_carteira_motorista : "";
            hasValue(pessoaFisica.dtaNascimento) ? dojo.byId("dtaNasci").value = pessoaFisica.dtaNascimento : "";
            hasValue(pessoaFisica.dtaCasamento) ? dojo.byId("dtaCasamento").value = pessoaFisica.dtaCasamento : "";
            hasValue(pessoaFisica.dc_num_titulo_eleitor) ? dojo.byId("tituloEleitor").value = pessoaFisica.dc_num_titulo_eleitor : "";
            hasValue(pessoaFisica.cd_loc_nascimento) ? dojo.byId('codLocalNascimento').value = pessoaFisica.cd_loc_nascimento : 0;
            pessoaFisica.txt_obs_pessoa != null ? dijit.byId('descObs').set('value', pessoaFisica.txt_obs_pessoa) : "";
            if (hasValue(pessoaFisica.LocalNascimento))
                hasValue(pessoaFisica.LocalNascimento.no_localidade) ? dojo.byId('descLocalNascimento').value = pessoaFisica.LocalNascimento.no_localidade : "";
            if (hasValue(pessoaFisica.EnderecoPrincipal))
                setValueEnderecoPrincipal(pessoaFisica.EnderecoPrincipal);
            if (hasValue(allContatosPrincipais))
                setValueContatosPrincipais(allContatosPrincipais, pessoaFisica.cd_telefone_principal);
            if (hasValue(allOutrosContatos))
                setarValueOutrosContatos(allOutrosContatos);
            if (hasValue(allEnderecos && allEnderecos.length > 0))
                setValueAllEnderecos(allEnderecos);
            if (hasValue(allRelacionamentos))
                setvalueRelacionamentos(allRelacionamentos);
            if (pessoaFisica.Atividade != null) {
                dojo.byId("atividadePrincipal").value = hasValue(pessoaFisica.Atividade.no_atividade) ? pessoaFisica.Atividade.no_atividade : "";
                dojo.byId("cdAtividade").value = hasValue(pessoaFisica.Atividade.cd_atividade) ? pessoaFisica.Atividade.cd_atividade : 0;
                dijit.byId('limparAtividade').set("disabled", false);
            }

            if (hasPessoaRelacionadaResponsavel() == true) {
                dijit.byId("pesEndResp").set("disabled", false);

            } else if (hasValue(dijit.byId("gridRelacionamentos"))) {
                dijit.byId("pesEndResp").set("disabled", true);
            }


            if (hasValue(pessoaFisica) && hasValue(pessoaFisica.ext_img_pessoa))
                showFoto({ cd_pessoa: pessoaFisica.cd_pessoa, ext_img_pessoa: pessoaFisica.ext_img_pessoa });
        }
        if (hasValue(carregando) && carregando)
            showCarregando();
        observeTitlePaneOpenUpdate();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function hasPessoaRelacionadaResponsavel() {
    try {
        var hasPessoa = false;

        if (hasValue(dijit.byId("gridRelacionamentos")) &&
            dijit.byId("gridRelacionamentos")['store'] != null &&
            dijit.byId("gridRelacionamentos")['store'] != undefined &&
            dijit.byId("gridRelacionamentos")['store']['objectStore'] != null &&
            dijit.byId("gridRelacionamentos")['store']['objectStore'] != undefined &&
            dijit.byId("gridRelacionamentos")['store']['objectStore']['data'] != null &&
            dijit.byId("gridRelacionamentos")['store']['objectStore']['data'] != undefined

        ) {
            if (dijit.byId("gridRelacionamentos").store.objectStore.data.length > 0) {
                hasPessoa = dijit.byId("gridRelacionamentos").store.objectStore.data.some(function (value) {
                    return value != null &&
                        value != undefined &&
                        value['relacionamento'] != null &&
                        value['relacionamento'] != undefined &&
                        (value['relacionamento'].cd_papel_filho == EnumPapelRelacionamento.RESPONSAVEL_FINANCEIRO ||
                            (value['relacionamento'].cd_papel_filho == EnumPapelRelacionamento.RESPONSAVEL_FINANCEIRO + "")) &&
                        (value['relacionamento'].cd_papel_pai == EnumPapelRelacionamento.ALUNO_RESPONSAVEL ||
                            (value['relacionamento'].cd_papel_pai == EnumPapelRelacionamento.ALUNO_RESPONSAVEL + "") ||
                            (value['relacionamento'].cd_papel_pai == "0")) &&
                        value.no_papel == "Responsavel Financeiro";

                });
            }
        }
        return hasPessoa;
    } catch (e) {
        postGerarLog(e);
    }
}

function setValueContatosPrincipais(contatos, cdTelefone) {
    try {
        if (contatos.length > 0)
            $.each(contatos, function (index, value) {
                if (value.cd_tipo_telefone == 4 && value.id_telefone_principal == true)
                    dojo.byId("email").value = value.dc_fone_mail;
                if (value.cd_tipo_telefone == 5 && value.id_telefone_principal == true)
                    dojo.byId("site").value = value.dc_fone_mail;
                if (value.cd_tipo_telefone == 1 && value.cd_telefone == cdTelefone)
                    dojo.byId("telefone").value = value.dc_fone_mail;
                if (value.cd_tipo_telefone == 3 && value.id_telefone_principal == true) {
                    dojo.byId("celular").value = value.dc_fone_mail;
                    dijit.byId("operadora").set("value", value.cd_operadora);
                    habilitaOperadora(dojo.byId("operadora"));
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValueEnderecoPrincipal(data) {
    try {
        configurarLayoutEndereco(LAYOUTPESQUISALOGRADOURO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
        var compEstado = dijit.byId("estado");
        var compCidade = dijit.byId("cidade");
        var compBairro = dijit.byId("bairro");
        var compCep = dijit.byId("cep");
        dojo.byId("cdEndereco").value = data.cd_endereco;
        //Estado
        if (hasValue(data.cd_loc_estado)) {
            compEstado._onChangeActive = false;
            compEstado.set("value", data.cd_loc_estado);
            compEstado.oldValue = data.cd_loc_estado;
            compEstado._onChangeActive = true
        }
        //Cidade
        if (hasValue(data.cd_loc_cidade)) {
            compCidade._onChangeActive = false;
            criarOuCarregarCompFiltering("cidade", [{ id: data.cd_loc_cidade, name: data.noLocCidade }], "", data.cd_loc_cidade, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            compCidade._onChangeActive = true;
        }
        //bairro
        if (hasValue(data.cd_loc_bairro)) {
            compBairro._onChangeActive = false;
            if (hasValue(data.bairros)) {
                criarOuCarregarCompFiltering("bairro", data.bairros, "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');

            } else
                criarOuCarregarCompFiltering("bairro", [{ id: data.cd_loc_bairro, name: data.noLocBairro }], "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            dijit.byId("bairro").oldValue = data.cd_loc_bairro;
            compBairro._onChangeActive = true
        }
        //Cep
        if (hasValue(data.num_cep)) {
            compCep._onChangeActive = false;
            compCep.set("value", data.num_cep);
            compCep._onChangeActive = true;
            compEstado.set("readOnly", true);
        }
        if (hasValue(data.cd_loc_logradouro)) {
            dojo.byId("codRua").value = data.cd_loc_logradouro;
            dijit.byId("rua").set("value", data.noLocRua);
        }
        if (hasValue(data.cd_tipo_logradouro))
            dijit.byId("logradouro").set("value", data.cd_tipo_logradouro);
        if (hasValue(data.cd_tipo_endereco))
            dijit.byId("tipoEndereco").set("value", data.cd_tipo_endereco);
        data.dc_compl_endereco != null ? dojo.byId("complemento").value = data.dc_compl_endereco : "";
        data.dc_num_endereco != null ? dojo.byId("numero").value = data.dc_num_endereco : "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarValueOutrosContatos(outrosContatos) {
    try {
        if (hasValue(dijit.byId("gridContatos")) && outrosContatos.length > 0) {
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: outrosContatos }) });
            dijit.byId("gridContatos").setStore(dataStore);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValueAllEnderecos(allEnderecos) {
    try {
        if (hasValue(dijit.byId("gridEnderecos")) && allEnderecos.length > 0) {
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: allEnderecos }) });
            dijit.byId("gridEnderecos").setStore(dataStore);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setvalueRelacionamentos(relacionamentos) {
    try {
        if (hasValue(dijit.byId("gridRelacionamentos")) && relacionamentos.length > 0) {
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: relacionamentos }) });
            dijit.byId("gridRelacionamentos").setStore(dataStore);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarExisitsEmpresaByCnpj() {
    try {
        var mensagensWeb = new Array();
        if ($("#cnpj").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/aluno/VerificarExistEmpresaByCnpjOrCdEmpresa?cnpj=" + $("#cnpj").val() + "&cdEmpresa=" + parseInt(0),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    apresentaMensagem(dojo.byId("descApresMsg").value, null);
                    if (data.retorno != null) {
                        setarValuePessoaJuridica(data.retorno, false);
                    } else {
                        $("#cd_pessoa").val(0);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "CNPJ válido.");
                        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(dojo.byId("descApresMsg").value, error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarEmpresaByCnpj() {
    try {
        var mensagensWeb = new Array();
        if ($("#inputRelac").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/pessoa/VerificarEmpresaByCnpj?cnpj=" + $("#inputRelac").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (data.retorno != null) {
                        dojo.byId("noPessoaRelac").value = data.retorno.no_pessoa;
                        dojo.byId("codPessoaRelac").value = data.retorno.cd_pessoa;
                        dijit.byId("sexoRelac").set("value", data.retorno.nm_sexo);
                        if (hasValue(data.retorno.EnderecoPrincipal))
                            setValueEnderecoPessoaRelac(data.retorno.EnderecoPrincipal);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresMsgNewRelac", error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//carrega os dados para uma nova pessoa
// Monta o tipo de endereço
function loadTipoEndereco(dataTipoEndereco, idComponente, Memory) {
    try {
        (hasValue(dataTipoEndereco.retorno)) ? dataTipoEndereco = dataTipoEndereco.retorno : dataTipoEndereco;

        var dados = [];
        $.each(dataTipoEndereco, function (index, val) {
            dados.push({
                "name": val.no_tipo_endereco,
                "id": val.cd_tipo_endereco
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId(idComponente).store = stateStore;
        //dijit.byId(idComponente).set("value", 2);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//carrega os dados para uma nova pessoa
// Monta o tipo de endereço
function loadOperadora(dataOperadora, idComponente, Memory) {
    try {
        (hasValue(dataOperadora.retorno)) ? dataOperadora = dataOperadora.retorno : dataOperadora;

        var dados = [];
        $.each(dataOperadora, function (index, val) {
            dados.push({
                "name": val.no_operadora,
                "id": val.cd_operadora
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId(idComponente).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Monta o Logradouro
function loadLogradouro(data, idComponente, Memory) {
    try {
        (hasValue(data.retorno)) ? data = data.retorno : data;

        var dados = [];
        $.each(data, function (index, val) {
            dados.push({
                "name": val.no_tipo_logradouro,
                "id": val.cd_tipo_logradouro
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId(idComponente).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Monta a Estado
function loadEstado(data, idComponente, Memory) {
    try {
        (hasValue(data.retorno)) ? data = data.retorno : data;

        var dados = [];
        var arrayEstado = new Array();
        $.each(data, function (index, value) {
            dados.push({
                'id': value.cd_localidade,
                'name': value.no_localidade,
                'sgl': value.sg_estado
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId(idComponente).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadClasseTelefone(dataClasseTelefone, Memory) {
    try {
        (hasValue(dataClasseTelefone)) ? dataClasseTelefone = dataClasseTelefone : dataClasseTelefone;

        var dados = [];
        $.each(dataClasseTelefone, function (index, val) {
            dados.push({
                "name": val.dc_classe_telefone,
                "id": val.cd_classe_telefone
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId("classeDialog").store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTipoTelefone(dataTipoTelefone, Memory) {
    try {

        (hasValue(dataTipoTelefone)) ? dataTipoTelefone = dataTipoTelefone : dataTipoTelefone;

        var dados = [];
        $.each(dataTipoTelefone, function (index, val) {
            dados.push({
                "name": val.no_tipo_telefone,
                "id": val.cd_tipo_telefone
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId("tipoContatoDialog").store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadNaturezaPessoaCadastro(Memory) {
    dojo.ready(function () {
        try {
            var stateStore = new Memory({
                data: [
                { name: "Física", id: "1" },
                { name: "Jurídica", id: "2" }
                ]
            });

            if (dijit.byId("naturezaPessoa") != null && typeof dijit.byId("naturezaPessoa") == typeof Object())
                dijit.byId("naturezaPessoa").store = stateStore;
            if (dijit.byId("naturezaPessoaRelac") != null && typeof dijit.byId("naturezaPessoa") == typeof Object())
                dijit.byId("naturezaPessoaRelac").store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadOrgaoExpedidor(dataOrgaoExpedidor, Memory) {
    try {
        (hasValue(dataOrgaoExpedidor.retorno)) ? dataOrgaoExpedidor = dataOrgaoExpedidor.retorno : dataOrgaoExpedidor;

        var dados = [];
        $.each(dataOrgaoExpedidor, function (index, val) {
            dados.push({
                "name": val.dc_abrev_orgao_expedidor,
                "id": val.cd_orgao_expedidor
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId("orgExp").store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadNacionalidade(dataPais, sexo, idNacionalidade) {
    try {
        var nacionalidade = dijit.byId("nacionalidade");
        (hasValue(jQuery.parseJSON(dataPais).retorno)) ? dataPais = jQuery.parseJSON(dataPais).retorno : jQuery.parseJSON(dataPais);

        var dados = [];
        if (sexo == FEMININO) {
            $.each(dataPais, function (index, val) {
                dados.push({
                    "id": val.cd_localidade,
                    "name": val.dc_nacionalidade_fem
                });
            });
        } else {
            $.each(dataPais, function (index, val) {
                dados.push({
                    "id": val.cd_localidade,
                    "name": val.dc_nacionalidade_masc
                });
            });
        }

        var stateStore = new dojo.store.Memory({
            data: eval(dados)
        });
        nacionalidade.store = stateStore;
        if (hasValue(idNacionalidade)) {
            nacionalidade.set("value", idNacionalidade);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTratamentoPessoa(dataTratamentoPessoa, sexo, idTratamento) {
    try {
        var tratamento = dijit.byId("tratamento");
        (hasValue(dataTratamentoPessoa.retorno)) ? dataTratamentoPessoa = dataTratamentoPessoa.retorno : dataTratamentoPessoa;

        var dados = [];
        if (sexo == FEMININO) {
            $.each(dataTratamentoPessoa, function (index, val) {
                dados.push({
                    "id": val.cd_tratamento,
                    "name": val.dc_tratamento_fem
                });
            });
        } else {
            $.each(dataTratamentoPessoa, function (index, val) {
                dados.push({
                    "id": val.cd_tratamento,
                    "name": val.dc_tratamento_masc
                });
            });
        }

        var stateStore = new dojo.store.Memory({
            data: eval(dados)
        });
        tratamento.store = stateStore;

        if (hasValue(idTratamento)) {
            tratamento.set("value", idTratamento);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEstadoCivil(dataEstadoCivil, sexo, idEstadoCivil) {
    try {
        var estadoCivil = dijit.byId("estadoCivil");
        (hasValue(dataEstadoCivil.retorno)) ? dataEstadoCivil = dataEstadoCivil.retorno : dataEstadoCivil;

        var dados = [];
        if (sexo == FEMININO) {
            $.each(dataEstadoCivil, function (index, val) {
                dados.push({
                    "id": val.cd_estado_civil,
                    "name": val.dc_estado_civil_fem
                });
            });
        } else {
            $.each(dataEstadoCivil, function (index, val) {
                dados.push({
                    "id": val.cd_estado_civil,
                    "name": val.dc_estado_civil_masc
                });
            });
        }

        var stateStore = new dojo.store.Memory({
            data: eval(dados)
        });
        estadoCivil.store = stateStore;
        if (hasValue(idEstadoCivil)) {
            estadoCivil.set("value", idEstadoCivil);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSexo(idSexo, Memory) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Feminino", id: 1 },
					{ name: "Masculino", id: 2 },
					{ name: "Não Binário", id: 3 },
					{ name: "Prefiro não responder ou Neutro", id: 4 }
            ]
        });
        dijit.byId(idSexo).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEstaOrgaoExpedidor(dataEtOrgExp, Memory) {
    try {
        (hasValue(dataEtOrgExp.retorno)) ? dataEtOrgExp = dataEtOrgExp.retorno : dataEtOrgExp;

        var dados = [];
        $.each(dataEtOrgExp, function (index, val) {
            dados.push({
                "name": val.sg_estado,
                "id": val.cd_localidade
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId("expRG").store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region - Métodos de auto complete
function mensagemPesquisaAutoComplete(idApresMsg) {
    try {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Nenhum registro encontrado para essa pesquisa.");
        apresentaMensagem(idApresMsg, mensagensWeb);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mensagemAutoComplete() {
    try {
        caixaDialogo(DIALOGO_AVISO, 'Nenhum registro encontrado para essa pesquisa.', '');
    }
    catch (e) {
        postGerarLog(e);
    }
}
// ============ fim dos métodos auto completes============\\
//#endregion

//Percistir cidade
function incluirCidade(defaultCidade, aprestMsg, idEstado, idCidade, descCidade) {
    try {
        var cidadeNova = trim(defaultCidade);
        if (hasValue(cidadeNova)) {
            apresentaMensagem('apresentadorMensagem', null);
            require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                xhr.post(Endereco() + "/api/localidade/PostCidade", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    data: ref.toJson({
                        no_localidade: cidadeNova,
                        cd_loc_relacionada: idEstado
                    })
                }).then(function (data) {
                    try {
                        var cdEstado = null;
                        if (hasValue(dijit.byId(idEstado).value)) {
                            cdEstado = dijit.byId(idEstado).value;
                        }
                        findCidadeByDesc(cidadeNova, idCidade, descCidade, aprestMsg, idEstado);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem(dojo.byId(aprestMsg).value, error);
                });
            });
        } else {
            apresentaMensagem(dojo.byId(aprestMsg).value, msgNotRegEnc);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirCidadeDialog(idCidade, apresMsg) {
    try {
        var cidadeNova = trim($("#setDefaultValueCidadeDialog").val());
        if (hasValue(cidadeNova)) {
            apresentaMensagem('apresentadorMensagem', null);
            require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                xhr.post(Endereco() + "/api/localidade/PostCidade", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    data: ref.toJson({
                        no_localidade: cidadeNova,
                        cd_loc_relacionada: dijit.byId('estadoOutrosEnd').value
                    })
                }).then(function (data) {
                    try {
                        // apresentaMensagem('apresentadorMensagemEscola', data);
                        var cdEstado = null;
                        if (hasValue(dijit.byId("estadoOutrosEnd").value)) {
                            cdEstado = dijit.byId("estadoOutrosEnd").value;
                        }
                        pesquisaCidade(cdEstado, "cidadeOutrosEnd", apresMsg);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem(apresMsg, error);
                });
            });
        } else {
            apresentaMensagem(apresMsg, msgNotRegEnc);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Montar diálogos axiliares para persistência do endereço
jQuery.fn.extend({
    propAttr: $.fn.prop || $.fn.attr
});

//Metodos de CRUD para o Dialog Outros Enderecos..
function incluirOutrosEndreços() {
    try {
        if (!dijit.byId("FormOutrosEndrecos").validate()) {
            return false;
        }

        var newOutrosEndrecos = {
            cd_endereco: 0,
            cd_loc_cidade: hasValue(dijit.byId("cidadeOutrosEnd").get("value")) ? dijit.byId("cidadeOutrosEnd").get("value") : 0,
            cd_loc_estado: hasValue(dijit.byId("estadoOutrosEnd").get("value")) ? dijit.byId("estadoOutrosEnd").get("value") : 0,
            cd_loc_pais: 1,
            cd_tipo_endereco: hasValue(dijit.byId("tipoEnderecoOutrosEnd").get("value")) ? dijit.byId("tipoEnderecoOutrosEnd").get("value") : 0,
            cd_tipo_logradouro: hasValue(dijit.byId("tipoLogradouroOutrosEnd").get("value")) ? dijit.byId("tipoLogradouroOutrosEnd").get('value') : 0,
            cd_loc_bairro: hasValue(dijit.byId("bairroOutrosEnd").get("value")) ? dijit.byId("bairroOutrosEnd").get("value") : 0,
            cd_loc_logradouro: hasValue(dojo.byId('codRuaOutrosEnd').value) ? dojo.byId('codRuaOutrosEnd').value : 0,
            dc_compl_endereco: hasValue(dijit.byId("complementoOutrosEnd").value) ? dijit.byId("complementoOutrosEnd").value : '',
            num_cep: hasValue(dojo.byId("cepOutrosEnd").value) ? dojo.byId("cepOutrosEnd").value : '',
            dc_num_endereco: hasValue(dojo.byId("numeroOutrosEnd").value) ? dojo.byId("numeroOutrosEnd").value : '',
            noLocBairro: hasValue(dijit.byId("bairroOutrosEnd").displayedValue) ? dijit.byId("bairroOutrosEnd").displayedValue : "",
            noLocCidade: hasValue(dijit.byId("cidadeOutrosEnd").displayedValue) ? dijit.byId("cidadeOutrosEnd").displayedValue : "",
            noLocRua: hasValue(dojo.byId("ruaOutrosEnd").value) ? dojo.byId("ruaOutrosEnd").value : ''
        };
        var dados = dijit.byId("gridEnderecos").store.objectStore.data;
        quickSortObj(dados, 'cd_tipo_endereco');
        var max_tam_dados = dados.length;
        insertObjSort(dados, "cd_tipo_endereco", newOutrosEndrecos, false);
        dijit.byId("gridEnderecos").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));

        var mensagensWeb = new Array();
        if (max_tam_dados === dados.length) 
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoTipoEnderecoDuplicado);
        else 
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegIncludSucess);
   
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("DialogEndereco").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarOutrosEnderecos() {
    try {
        if (!dijit.byId("FormOutrosEndrecos").validate()) {
            return false;
        }
        var enderecos = dijit.byId("gridEnderecos").selection.getSelected();
        if (enderecos.length > 0) {
            enderecos[0].cd_endereco = 0,
            enderecos[0].cd_loc_cidade = hasValue(dijit.byId("cidadeOutrosEnd").get("value")) ? dijit.byId("cidadeOutrosEnd").get("value") : 0,
            enderecos[0].cd_loc_estado = hasValue(dijit.byId("estadoOutrosEnd").get("value")) ? dijit.byId("estadoOutrosEnd").get("value") : 0,
            enderecos[0].cd_loc_pais = 1,
            enderecos[0].cd_tipo_endereco = hasValue(dijit.byId("tipoEnderecoOutrosEnd").get("value")) ? dijit.byId("tipoEnderecoOutrosEnd").get("value") : 0,
            enderecos[0].cd_tipo_logradouro = hasValue(dijit.byId("tipoLogradouroOutrosEnd").get("value")) ? dijit.byId("tipoLogradouroOutrosEnd").get('value') : 0,
            enderecos[0].cd_loc_bairro = hasValue(dijit.byId("bairroOutrosEnd").get("value")) ? dijit.byId("bairroOutrosEnd").get("value") : 0,
            enderecos[0].cd_loc_logradouro = hasValue(dojo.byId('codRuaOutrosEnd').value) ? dojo.byId('codRuaOutrosEnd').value : 0,
            enderecos[0].dc_compl_endereco = hasValue(dijit.byId("complementoOutrosEnd").value) ? dijit.byId("complementoOutrosEnd").value : '',
            enderecos[0].num_cep = hasValue(dojo.byId("cepOutrosEnd").value) ? dojo.byId("cepOutrosEnd").value : '',
            enderecos[0].dc_num_endereco = hasValue(dojo.byId("numeroOutrosEnd").value) ? dojo.byId("numeroOutrosEnd").value : '',
            enderecos[0].noLocBairro = hasValue(dijit.byId("bairroOutrosEnd").displayedValue) ? dijit.byId("bairroOutrosEnd").displayedValue : "",
            enderecos[0].noLocCidade = hasValue(dijit.byId("cidadeOutrosEnd").displayedValue) ? dijit.byId("cidadeOutrosEnd").displayedValue : "",
            enderecos[0].noLocRua = hasValue(dojo.byId("ruaOutrosEnd").value) ? dojo.byId("ruaOutrosEnd").value : ''
        }
        dijit.byId("gridEnderecos").update();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegAltSucess);
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("DialogEndereco").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepVelusOutrosEnderecos() {
    try {
        var enderecos = dijit.byId("gridEnderecos").selection.getSelected();
        if (enderecos.length > 0) {
            dojo.byId("complementoOutrosEnd").value = enderecos[0].dc_compl_endereco;
            dojo.byId("cepOutrosEnd").value = enderecos[0].num_cep;
            dojo.byId("numeroOutrosEnd").value = enderecos[0].dc_num_endereco;
            if (hasValue(enderecos[0].cd_tipo_endereco))
                dijit.byId("tipoEnderecoOutrosEnd").set("value", enderecos[0].cd_tipo_endereco);
            configurarLayoutEndereco(LAYOUTPESQUISACEP, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd",
                                            "ruaOutrosEnd", "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
            setValuesEndereco(enderecos[0], "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd", "ruaOutrosEnd", "codRuaOutrosEnd", "cepOutrosEnd", "tipoLogradouroOutrosEnd");
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparOutrosEnderecos() {
    try {
        var compEstado = dijit.byId("estadoOutrosEnd");
        var compCidade = dijit.byId("cidadeOutrosEnd");
        var compBairro = dijit.byId("bairroOutrosEnd");

        clearForm("FormOutrosEndrecos");
        dojo.byId("cdEnderecoOutrosEnd").value = 0;
        compEstado._onChangeActive = false;
        compEstado.reset();
        compEstado.oldValue = 0;
        compEstado._onChangeActive = true;
        compCidade._onChangeActive = false;
        compCidade.reset();
        compBairro._onChangeActive = true;
        compBairro._onChangeActive = false;
        compBairro.reset();
        dijit.byId("tipoEnderecoOutrosEnd").reset();
        dijit.byId("tipoLogradouroOutrosEnd").reset();
        dijit.byId("ruaOutrosEnd").reset();
        dojo.byId("codRuaOutrosEnd").value = 0;
        dojo.byId("numeroOutrosEnd").value = "";
        dojo.byId("complementoOutrosEnd").value = "";
        dojo.byId("cepOutrosEnd").value = "";
        dijit.byId("cepOutrosEnd").reset();
        configurarLayoutEndereco(LAYOUTPADRAO, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd", "ruaOutrosEnd", "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirOutrosEnd() {
    require(["dojo/ready", "dojo/store/Memory", "dojo/data/ObjectStore"],
     function (Memory, ObjectStore) {
         try {
             var enderecos = dijit.byId("gridEnderecos").selection.getSelected();

             if (enderecos.length > 0) {
                 var arrayEnderecos = dijit.byId("gridEnderecos")._by_idx;
                 arrayEnderecos = jQuery.grep(arrayEnderecos, function (value) {
                     return value.item != enderecos[0];
                 });
                 var dados = [];
                 $.each(arrayEnderecos, function (index, value) {
                     dados.push(value.item);
                 });
                
                 dijit.byId("gridEnderecos").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
                 
                 var mensagensWeb = new Array();
                 mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegExcludSucess);
                 apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                 dijit.byId("DialogEndereco").hide();
             }
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}

//Fim metodos (Outros Endereços)

function limparOutrosContatos() {
    try {
        clearForm("FormOutrosContatos");
        dijit.byId("tipoContatoDialog").reset();
        dijit.byId("operadoraDialog").reset();
        dijit.byId("classeDialog").reset();
        dojo.byId('cdTelefone').value = 0;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirOutrosContatos() {
    try {
        if (!dijit.byId("FormOutrosContatos").validate()) {
            return false;
        }
        var newOutrosContatos = {
            cd_teletefone: 0,
            cd_tipo_telefone: hasValue(dijit.byId("tipoContatoDialog").get("value")) ? dijit.byId("tipoContatoDialog").get("value") : 0,
            cd_classe_telefone: hasValue(dijit.byId("classeDialog").get("value")) ? dijit.byId("classeDialog").get("value") : 0,
            dc_fone_mail: dojo.byId("descTelefoneDialog").value,
            des_tipo_contato: dijit.byId("tipoContatoDialog").displayedValue,
            no_classe: dijit.byId("classeDialog").displayedValue,
            cd_operadora: hasValue(dijit.byId("operadoraDialog").get("value")) ? dijit.byId("operadoraDialog").get("value") : null
        }
        dijit.byId("gridContatos").store.newItem(newOutrosContatos);
        dijit.byId("gridContatos").store.save();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Contato incluido com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("dialogContato").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepVelusOutrosContatos() {
    try {
        var contatos = dijit.byId("gridContatos").selection.getSelected();

        if (contatos.length > 0) {
            dijit.byId("tipoContatoDialog").set("value", contatos[0].cd_tipo_telefone);
            dijit.byId("operadoraDialog").set("value", contatos[0].cd_operadora);
            dijit.byId("classeDialog").set("value", contatos[0].cd_classe_telefone);
            //dojo.byId("tipoContatoDialog").value = contatos[0].des_tipo_contato;
            dojo.byId("classeDialog").value = contatos[0].no_classe;
            dojo.byId("descTelefoneDialog").value = contatos[0].dc_fone_mail;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirOutrosContatos() {
    try {
        var contatos = dijit.byId("gridContatos").selection.getSelected();
        if (contatos.length > 0) {
            var arrayContatos = dijit.byId("gridContatos")._by_idx;
            arrayContatos = jQuery.grep(arrayContatos, function (value) {
                return value.item != contatos[0];
            });
            var dados = [];
            $.each(arrayContatos, function (index, value) {
                dados.push(value.item);
            });
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
            dijit.byId("gridContatos").setStore(dataStore);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Contato excluido com sucesso.");
            apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
            dijit.byId("dialogContato").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function altararOutrosContatos() {
    try {
        if (!dijit.byId("FormOutrosContatos").validate()) {
            return false;
        }
        var contatos = dijit.byId("gridContatos").selection.getSelected();
        if (contatos.length > 0) {
            contatos[0].cd_teletefone = dojo.byId("cdTelefone").value,
            contatos[0].cd_tipo_telefone = hasValue(dijit.byId("tipoContatoDialog").get("value")) ? dijit.byId("tipoContatoDialog").get("value") : 0,
            contatos[0].cd_classe_telefone = hasValue(dijit.byId("classeDialog").get("value")) ? dijit.byId("classeDialog").get("value") : 0,
            contatos[0].no_classe = dijit.byId("classeDialog").displayedValue,
            contatos[0].des_tipo_contato = dijit.byId("tipoContatoDialog").displayedValue,
            contatos[0].dc_fone_mail = dojo.byId("descTelefoneDialog").value,
            contatos[0].cd_operadora = hasValue(dijit.byId("operadoraDialog").get("value")) ? dijit.byId("operadoraDialog").get("value") : null
        }
        dijit.byId("gridContatos").update();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Contato alterado com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("dialogContato").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos de Crud Relacionamento
function loadDropDownRelac(dataPapel) {
    require([
     "dojo/ready",
     "dijit/DropDownMenu",
     "dijit/MenuItem"
    ], function (ready, DropDownMenu, MenuItem) {
        ready(function () {
            try {
                var menuRelac = new DropDownMenu();

                $.each(dataPapel, function (index, value) {
                    if (value.cd_papel > 0) {
                        // Configura o cd_papel_pai para salvar logo depois no banco:
                        if (hasValue(value.PapeisPais) && value.PapeisPais.length > 0)
                            value.cd_papel_pai = value.PapeisPais[0].cd_papel;

                        var item = new MenuItem({
                            value: value,
                            label: value.no_papel,
                            onClick: function () {
                                openModalRelac(this.value);
                            }
                        });
                        menuRelac.addChild(item);
                    }
                });
                dijit.byId("btnNovoRelac").dropDown = menuRelac;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function openModalRelac(value) {
    try {
        limparNewRelac();
        dojo.byId("nmNaturezaPapel").value = "";
        IncluirAlterar(1, 'divAlterarPessoaRelac', 'divIncluirPessoaRelac', 'divExcluirPessoaRelac', dojo.byId("descApresMsg").value, 'divCancelarPessoaRelac', 'divCleanPessoaRelac');
        dojo.byId("codPapelRelac").value = value.cd_papel;
        dojo.byId("noPapel").value = value.no_papel;

        dojo.byId("nmNaturezaPapel").value = value.id_natureza_papel;
        dijit.byId("naturezaPessoaRelac").set("value", value.id_natureza_papel);
        document.getElementById('divNaturezaRel').style.display = 'none';
        dijit.byId('noPessoaRelac').attr("disabled", false);

        configuraLayoutRelacionamento(value);
        dojo.byId("DialogRelac_title").innerHTML = "Pessoa Relacionada (" + value.no_papel + ")";
        dijit.byId("DialogRelac").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraLayoutRelacionamento(value) {
    try {
        if (value.id_natureza_papel == PAPELCPF) {
            dojo.byId("laCPfCnpj").innerHTML = "CPF:";
            $("#inputRelac").mask("999.999.999-99");
            dojo.byId('tdSexo').style.display = '';
            dojo.byId('tdSexo2').style.display = '';
        } else if (value.id_natureza_papel == PAPELCNPJ) {
            dojo.byId("laCPfCnpj").innerHTML = "CNPJ:";
            $("#inputRelac").mask("99.999.999/9999-99");
            dojo.byId('tdSexo').style.display = 'none';
            dojo.byId('tdSexo2').style.display = 'none';
        } else if (value.id_natureza_papel == PAPELCPFCNPJ) {
            dojo.byId("laCPfCnpj").innerHTML = "CPF:";
            $("#inputRelac").mask("999.999.999-99");
            document.getElementById('divNaturezaRel').style.display = '';
            dijit.byId("naturezaPessoaRelac").set("value", PESSOAFISICA);
            dojo.byId('tdSexo').style.display = '';
            dojo.byId('tdSexo2').style.display = '';
        }
        if (value.cd_papel == PAPEL_RESPONSAVEL) {
            dojo.byId('lblGrauParent').style.display = '';
            dojo.byId('tdGrauParent').style.display = '';
        }
        else {
            dojo.byId('lblGrauParent').style.display = 'none';
            dojo.byId('tdGrauParent').style.display = 'none';
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarPessoaFisicaRelac() {
    try {
        var retorno = null;
        var codPessoaDepRelac = null;
        var cpf = "";
        if (hasValue(dojo.byId("codPessoaDepRelac").value) && dojo.byId("codPessoaDepRelac").value > 0)
            cpf = dojo.byId("nmCPFPessoaQueUSo").value;
        else
            cpf = dojo.byId("inputRelac").value;

        retorno = {
            no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
            no_papel: hasValue(dojo.byId("noPapel").value) ? dojo.byId("noPapel").value : "",
            nm_tipo_papel: dojo.byId("nmTipoPapel").value,
            cpfCnpj: cpf,
            desc_qualif_relacionamento : dijit.byId("grauParent").displayedValue,
            dc_fone_mail: hasValue(dijit.byId("telefonePessoaRelac").value) ? dijit.byId("telefonePessoaRelac").value.replace("(__) ____-____", "") : "",
            //dc_fone_mail_relac: hasValue(dijit.byId("emailRelac").value) ? dijit.byId("emailRelac").value : "",
            relacionamento: {
                cd_pessoa_pai: hasValue(dojo.byId("cd_pessoa").value) ? dojo.byId("cd_pessoa").value : 0,
                cd_pessoa_filho: hasValue(codPessoaRelac.value) ? codPessoaRelac.value : 0,
                cd_papel_filho: hasValue(dojo.byId("codPapelRelac").value) ? dojo.byId("codPapelRelac").value : 0,
                cd_papel_pai: hasValue(dojo.byId("codPapelPaiRelac").value) ? dojo.byId("codPapelPaiRelac").value : null,
                id_natureza_papel: hasValue(dijit.byId("naturezaPessoaRelac").value) ? parseInt(dijit.byId("naturezaPessoaRelac").value) : 0,
                cd_qualif_relacionamento: hasValue(dijit.byId("grauParent").value) ? dijit.byId("grauParent").value : null
            },
            pessoaFisicaRelac: {
                nm_sexo: hasValue(dijit.byId("sexoRelac").get("value")) ? dijit.byId("sexoRelac").get("value") : null,
                nm_cpf: hasValue(dojo.byId("inputRelac").value) ? dojo.byId("inputRelac").value : null,
                no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
                cd_pessoa_cpf: hasValue(dojo.byId("codPessoaDepRelac").value) ? dojo.byId("codPessoaDepRelac").value : null
            },
            id_natureza_papel: 1,
            nm_natureza_pessoa: 1,
            ehSelectBd: false,
            ehRegBdEdit: false
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDataPessoaJuridica() {
    try {
        var retorno = null;
        retorno = {
            no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
            no_papel: hasValue(dojo.byId("noPapel").value) ? dojo.byId("noPapel").value : "",
            nm_tipo_papel: dojo.byId("nmTipoPapel").value,
            cpfCnpj: hasValue(dojo.byId("inputRelac").value) ? dojo.byId("inputRelac").value : "",
            dc_fone_mail: hasValue(dijit.byId("telefonePessoaRelac").value) ? dijit.byId("telefonePessoaRelac").value : "",
            //dc_fone_mail_relac: hasValue(dijit.byId("emailRelac").value) ? dijit.byId("emailRelac").value : "",
            relacionamento: {
                cd_pessoa_pai: hasValue(dojo.byId("cd_pessoa").value) ? dojo.byId("cd_pessoa").value : 0,
                cd_pessoa_filho: hasValue(codPessoaRelac.value) ? codPessoaRelac.value : 0,
                cd_papel_filho: hasValue(dojo.byId("codPapelRelac").value) ? dojo.byId("codPapelRelac").value : 0,
                cd_papel_pai: hasValue(dojo.byId("codPapelPaiRelac").value) ? dojo.byId("codPapelPaiRelac").value : null,
                id_natureza_papel: hasValue(dijit.byId("naturezaPessoaRelac").value) ? parseInt(dijit.byId("naturezaPessoaRelac").value) : 0
            },
            pessoaJuridicaRelac: {
                no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
                dc_num_cgc: hasValue(dojo.byId("inputRelac").value) ? dojo.byId("inputRelac").value : null
            },
            nm_natureza_pessoa: 2,
            id_natureza_papel: 2,
            ehSelectBd: false,
            ehRegBdEdit: false
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirNewRelac() {
    try {
        var codPessoaRelac = dojo.byId("codPessoaRelac");
        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        var mensagensWeb = new Array();

        if (codPessoaRelac.value <= 0 && dojo.byId("codPessoaDepRelac").value <= 0 || dojo.byId("codPessoaDepRelac").value <= 0 && codPessoaRelac.value <= 0)
            if (!dijit.byId("formCpfCnpjRelac").validate())
                return false;
        if (dijit.byId("naturezaPessoaRelac").value == PESSOAFISICA)
            if (!dijit.byId("formSexoRelac").validate())
                return false;
        if (!dijit.byId("formNoPessoaRelac").validate())
            return false;
        if (hasValue(dojo.byId("codPapelRelac")) && parseInt(dojo.byId("codPapelRelac").value) == 3) { // 3 é o ID do papel "RESPONSAVEL".
            if (!dijit.byId("formGrauParent").validate())
                return false;
        }
        
        var newRelac = null;
        if (hasValue(dojo.byId("nmNaturezaPapel").value) && dojo.byId("nmNaturezaPapel").value == PESSOAFISICA)
            newRelac = montarPessoaFisicaRelac();
        if (hasValue(dojo.byId("nmNaturezaPapel").value) && dojo.byId("nmNaturezaPapel").value == PESSOAJURIDICA)
            newRelac = montarDataPessoaJuridica();
        if (hasValue(dojo.byId("nmNaturezaPapel").value) && dojo.byId("nmNaturezaPapel").value == 0) {
            if (hasValue(dijit.byId("naturezaPessoaRelac").value) && dijit.byId("naturezaPessoaRelac").value == PESSOAFISICA)
                newRelac = montarPessoaFisicaRelac();
            if (hasValue(dijit.byId("naturezaPessoaRelac").value) && dijit.byId("naturezaPessoaRelac").value == PESSOAJURIDICA)
                newRelac = montarDataPessoaJuridica();
        }
        if (newRelac != null && (newRelac.relacionamento == null || newRelac.relacionamento.cd_papel_filho <= 0)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Não foi possível incluir o Relacionamento, Não esta configurado o papel pai.");
            apresentaMensagem('apresMsgNewRelac', mensagensWeb);
            return false;
        }

        if (hasValue(dijit.byId("gridRelacionamentos")) &&
            hasValue(dijit.byId("gridRelacionamentos").store.objectStore.data)) {
            var hasRelacionamentoRepetido = dijit.byId("gridRelacionamentos").store.objectStore.data.some(
                function(element) {

                    return (element != null &&
                        element != undefined &&
                        element['relacionamento'] != null &&
                        element['relacionamento'] != undefined &&
                        element.relacionamento.cd_papel_filho == "18" &&
                        element.relacionamento.cd_pessoa_filho == newRelac.relacionamento.cd_pessoa_filho &&
                        element.relacionamento.cd_pessoa_pai == newRelac.relacionamento.cd_pessoa_pai
                    );
                });
            if (hasRelacionamentoRepetido) {
                mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Este relacionamento já existe na grade.");
                apresentaMensagem('apresMsgNewRelac', mensagensWeb);
                return false;
            }
        }

        if (hasValue(newRelac)) {
            dijit.byId("gridRelacionamentos").store.newItem(newRelac);
            dijit.byId("gridRelacionamentos").store.save();

            if (hasPessoaRelacionadaResponsavel() == true) {
                dijit.byId("pesEndResp").set("disabled", false);

            } else if (hasValue(dijit.byId("gridRelacionamentos"))) {
                dijit.byId("pesEndResp").set("disabled", true);
            }

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Relacionamento incluído com sucesso.");
            apresentaMensagem('descApresMsg', mensagensWeb);
            dijit.byId("DialogRelac").hide();
        } else {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Não foi possível incluir o Relacionamento.");
            apresentaMensagem(dojo.byId("apresMsgNewRelac").value, mensagensWeb);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparNewRelac() {
    try {
        clearForm("formCpfCnpjRelac");
        clearForm("formDepRelac");
        clearForm("formNoPessoaRelac");
        clearForm("formSexoRelac");
        dojo.byId("codPessoaRelac").value = 0;
        dojo.byId("codPessoaDepRelac").value = 0;
        dojo.byId("codPapelRelac").value = 0;
        //dojo.byId("nmNaturezaPapel").value = "";
        dojo.byId("codPapelPaiRelac").value = 0;
        dijit.byId("sexoRelac").reset();
        dijit.byId("sexoRelac").set("disabled", false);
        dijit.byId("inputRelac").set("disabled", false);
        dijit.byId("noPessoaRelac").set("disabled", false);
        dijit.byId("telefonePessoaRelac").set("disabled", false);
        dijit.byId("grauParent").reset();
        dijit.byId("telefonePessoaRelac").reset();
    //    dijit.byId("emailRelac").set("disabled", false);
    //    dijit.byId("emailRelac").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirRelac() {
    try {
        var value = null;
        var grid = dijit.byId("gridRelacionamentos");
        var itensSelecionados = montarRelacSelecionados();
        var codPessoaRelac = dojo.byId("codPessoaRelac");
        var ehLink = document.getElementById('ehLinkEdit').value;

        if (eval(ehLink) == false) {
            value = grid.selection.getSelected();
        } else {
            value = itensSelecionados;
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
                return false;
            }
        }
        var itensSelecionado = value;
        var dados = [];

        var arrayRelacionamentos = [];
        if (hasValue(dijit.byId("gridRelacionamentos")) && hasValue(dijit.byId("gridRelacionamentos").store.objectStore.data)) {
            arrayRelacionamentos = dijit.byId("gridRelacionamentos").store.objectStore.data;
        }
        
        if (arrayRelacionamentos.length > 0) {
            
            if (itensSelecionado.length > 0) {
                $.each(itensSelecionado, function (idx, valueItemSelecionado) {
                    
                    var pos = arrayRelacionamentos.indexOf(valueItemSelecionado);
                    if (pos > -1) {
                        arrayRelacionamentos.splice(pos, 1);
                    }
                });

                $.each(arrayRelacionamentos, function (index, valueRelacionamentos) {
                    dados.push(valueRelacionamentos);
                });
            }
        }
        


        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
        dijit.byId("gridRelacionamentos").setStore(dataStore);
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Relacionamento excluido com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("DialogRelac").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValuePessoaRelac(dataRelac, ehRelacInverso) {
    try {
        if (ehRelacInverso) {

            dojo.byId("codPessoaRelac").value = dataRelac.cd_pessoa_pai;
            dojo.byId("codPapelPaiRelac").value = dataRelac.cd_papel_pai;
            dojo.byId("codPapelRelac").value = dataRelac.cd_papel_filho;
            dojo.byId("cdRelacionamento").value = dataRelac.cd_relacionamento;

            if (dataRelac.PessoaPai != null) {
                dojo.byId("noPessoaRelac").value = dataRelac.PessoaPai.no_pessoa;
                dijit.byId("naturezaPessoaRelac").value = dataRelac.PessoaPai.nm_natureza_pessoa;
            }
            if (dataRelac.PapelPai != null) {
                dojo.byId("noPapel").value = dataRelac.PapelPai.no_papel;
                dojo.byId("DialogRelac_title").innerHTML = "Pessoa Relacionada (" + dataRelac.PapelPai.no_papel + ")";
                configuraLayoutRelacionamento(dataRelac.PapelPai);
                dojo.byId("nmNaturezaPapel").value = dataRelac.PapelPai.id_natureza_papel;
                dojo.byId("nmTipoPapel").value = dataRelac.PapelPai.nm_tipo_papel;
            }

            if (dataRelac.PessoaPai.nm_natureza_pessoa == PESSOAFISICA) {
                dijit.byId("sexoRelac").set("value", dataRelac.PessoaPai.nm_sexo);
                dojo.byId("inputRelac").value = dataRelac.PessoaPai.nm_cpf;
            }
            if (dataRelac.PessoaPai.nm_natureza_pessoa == PESSOAJURIDICA) {
                dojo.byId("inputRelac").value = dataRelac.PessoaPai.dc_num_cgc;
            }
            if (dataRelac.PessoaPai.EnderecoPrincipal != null)
                setValueEnderecoPessoaRelac(dataRelac.PessoaPai.EnderecoPrincipal);

        } else {
            dojo.byId("codPessoaRelac").value = dataRelac.cd_pessoa_filho;
            dojo.byId("codPapelRelac").value = dataRelac.cd_papel_filho;
            dojo.byId("codPapelPaiRelac").value = dataRelac.cd_papel_pai;
            dojo.byId("cdRelacionamento").value = dataRelac.cd_relacionamento;

            if (dataRelac.PessoaFilho != null) {
                dojo.byId("noPessoaRelac").value = dataRelac.PessoaFilho.no_pessoa;
                dijit.byId("naturezaPessoaRelac").value = dataRelac.PessoaFilho.nm_natureza_pessoa;
            }
            if (dataRelac.PapelFilho != null) {
                dojo.byId("noPapel").value = dataRelac.PapelFilho.no_papel;
                dojo.byId("DialogRelac_title").innerHTML = "Pessoa Relacionada (" + dataRelac.PapelFilho.no_papel + ")";
                dojo.byId("nmNaturezaPapel").value = dataRelac.PapelFilho.id_natureza_papel;
                configuraLayoutRelacionamento(dataRelac.PapelFilho);
                dojo.byId("nmTipoPapel").value = dataRelac.PapelFilho.nm_tipo_papel;
            }
            if (dataRelac.PessoaFilho.nm_natureza_pessoa == PESSOAFISICA) {
                dijit.byId("sexoRelac").set("value", dataRelac.PessoaFilho.nm_sexo);
                dojo.byId("inputRelac").value = dataRelac.PessoaFilho.nm_cpf;
            }
            if (dataRelac.PessoaFilho.nm_natureza_pessoa == PESSOAJURIDICA) {
                //dojo.byId("nmNaturezaPapel").set("value", PESSOAJURIDICA);
                dojo.byId("inputRelac").value = dataRelac.PessoaFilho.dc_num_cgc;
            }
            if (dataRelac.PessoaFilho.EnderecoPrincipal != null)
                setValueEnderecoPessoaRelac(dataRelac.PessoaFilho.EnderecoPrincipal);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValueEnderecoPessoaRelac(endereco) {
    try {
        //codCidadeRelac = endereco.cd_loc_cidade;
        dojo.byId("codCidadeRelacDialog").value = endereco.cd_loc_cidade;

        dijit.byId("estadoRelac").set("value", endereco.cd_loc_estado);
        dijit.byId("tipoEnderecoRelac").set("value", endereco.cd_tipo_endereco);
        dijit.byId("logradouroRelac").set('value', endereco.cd_tipo_logradouro);

        dojo.byId("cdEnderecoRelac").value = endereco.cd_endereco;
        dojo.byId('codBairroRelac').value = endereco.cd_loc_bairro;
        dojo.byId('codRuaRelac').value = endereco.cd_loc_logradouro,
        dojo.byId("complementoRelac").value = endereco.dc_compl_endereco;
        dojo.byId("cepRelac").value = endereco.num_cep;
        dojo.byId("numeroRelac").value = endereco.dc_num_endereco;
        if (endereco.EnderecoLocBairro != null)
            dojo.byId("bairroRelac").value = endereco.EnderecoLocBairro.no_localidade;
        else
            dojo.byId("bairroRelac").value = endereco.noLocBairro;
        if (endereco.EnderecoLocLogradouro != null)
            dojo.byId("ruaRelac").value = endereco.EnderecoLocLogradouro.no_localidade;
        else
            dojo.byId("ruaRelac").value = endereco.noLocRua;

        if (hasValue(endereco.EnderecoLocCidade) && endereco.EnderecoLocCidade.no_localidade != null)
            dojo.byId("cidadeRelac").value = endereco.EnderecoLocCidade.no_localidade;
        else
            dojo.byId("cidadeRelac").value = endereco.noLocCidade;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxRelac(value, rowIndex, obj) {
    try {
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input  id='" + id + "' /> ";

        setTimeout("configuraCheckBoxRelac(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;

    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxRelac(value, rowIndex, id) {
    try {
        var dojoId = dojo.byId(id);

        if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
            dijit.byId(id).destroy();

        require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
            ready(function () {
                try {
                    if (hasValue(dojoId) && dojoId.type == 'text')
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            checked: value,
                            onChange: function (b) { checkBoxChangeRelac(rowIndex) }
                        }, id);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeRelac(rowIndex) {
    try {
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        gridRelacionamentos._by_idx[rowIndex].item.ehSelecionado = !gridRelacionamentos._by_idx[rowIndex].item.ehSelecionado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarRelacionamentosCompleto() {
    try {
        var relacionamentos = [];
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        var data = new Array();

        hasValue(gridRelacionamentos) ? gridRelacionamentos.store.save() : null;
        if (hasValue(gridRelacionamentos) && hasValue(gridRelacionamentos.store.objectStore.data))
            data = dijit.byId("gridRelacionamentos").store.objectStore.data;
        else
            relacionamentos = null;
        if (hasValue(gridRelacionamentos) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                relacionamentos.push({
                    cpfCnpj: val.nm_tipo_papel,
                    no_pessoa: val.no_pessoa,
                    no_papel: val.no_papel,
                    id_natureza_papel: val.id_natureza_papel,
                    nm_natureza_pessoa: val.nm_natureza_pessoa,
                    ehSelectBd: val.ehSelectBd,
                    ehRegBdEdit: val.ehRegBdEdit,
                    ehRelacInverso: val.ehRelacInverso,
                    dc_fone_mail: val.dc_fone_mail,
                    //no_pessoa_dependente: val.no_pessoa_dependente,
                    //nm_tipo_papel: val.nm_tipo_papel,
                    relacionamento: {
                        cd_relacionamento: val.relacionamento.cd_relacionamento,
                        cd_pessoa_pai: val.relacionamento.cd_pessoa_pai,//hasValue(dojo.byId("cd_pessoa").value) ? dojo.byId("cd_pessoa").value : 0,
                        cd_pessoa_filho: val.relacionamento.cd_pessoa_filho,//hasValue(codPessoaRelac.value) ? codPessoaRelac.value : 0,
                        cd_papel_filho: val.relacionamento.cd_papel_filho,//hasValue(dojo.byId("codPapelRelac").value) ? dojo.byId("codPapelRelac").value : 0,
                        cd_papel_pai: val.relacionamento.cd_papel_pai,//hasValue(dojo.byId("codPapelPaiRelac").value) ? dojo.byId("codPapelPaiRelac").value : null,
                        id_natureza_papel: val.relacionamento.id_natureza_papel,//hasValue(dijit.byId("naturezaPessoaRelac").value) ? parseInt(dijit.byId("naturezaPessoaRelac").value) : 0
                        cd_qualif_relacionamento: val.relacionamento.cd_qualif_relacionamento
                    },
                    pessoaFisicaRelac: val.pessoaFisicaRelac,/*{
                    nm_sexo: hasValue(dijit.byId("sexoRelac").get("value")) ? dijit.byId("sexoRelac").get("value") : null,
                    nm_cpf: hasValue(dojo.byId("inputRelac").value) ? dojo.byId("inputRelac").value : "",
                    no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
                    cd_pessoa_cpf: hasValue(dojo.byId("codPessoaDepRelac").value) ? dojo.byId("codPessoaDepRelac").value : null
                },*/
                    pessoaJuridicaRelac: val.pessoaJuridicaRelac/*{
                    no_pessoa: hasValue(dojo.byId("noPessoaRelac").value) ? dojo.byId("noPessoaRelac").value : "",
                    dc_num_cgc: hasValue(dojo.byId("inputRelac").value) ? dojo.byId("inputRelac").value : ""
                },*/
                });
            });
        return relacionamentos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarRelacionamentos() {
    try {
        var relacionamentos = [];
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        var data = null;

        hasValue(gridRelacionamentos) ? gridRelacionamentos.store.save() : null;
        if (hasValue(gridRelacionamentos) && hasValue(gridRelacionamentos.store.objectStore.data))
            data = dijit.byId("gridRelacionamentos").store.objectStore.data;
        else
            relacionamentos = null;
        if (hasValue(gridRelacionamentos) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                if (!val.ehSelectBd) {
                    if (hasValue(val.nm_natureza_pessoa) && val.nm_natureza_pessoa == 1) {
                        relacionamentos.push({
                            relacionamento: val.relacionamento,
                            pessoaFisicaRelac: val.pessoaFisicaRelac,
                            nm_natureza_pessoa: val.nm_natureza_pessoa
                        });
                    } else {
                        relacionamentos.push({
                            relacionamento: val.relacionamento,
                            pessoaJuridicaRelac: val.pessoaJuridicaRelac,
                            nm_natureza_pessoa: val.nm_natureza_pessoa
                        });
                    }
                } else {
                    if (val.ehRegBdEdit)
                        if (val.nm_natureza_pessoa == PESSOAFISICA)
                            relacionamentos.push({
                                relacionamento: val.relacionamento,
                                pessoaFisicaRelac: val.pessoaFisicaRelac,
                                nm_natureza_pessoa: val.nm_natureza_pessoa
                            });
                        else
                            relacionamentos.push({
                                relacionamento: val.relacionamento,
                                pessoaJuridicaRelac: val.pessoaJuridicaRelac,
                                nm_natureza_pessoa: val.nm_natureza_pessoa
                            });

                    else
                        relacionamentos.push({
                            relacionamento: {
                                cd_relacionamento: val.relacionamento.cd_relacionamento,
                                cd_pessoa_pai: val.relacionamento.cd_pessoa_pai,
                                cd_pessoa_filho: val.relacionamento.cd_pessoa_filho,
                                cd_papel_filho: val.relacionamento.cd_papel_filho,
                                cd_papel_pai: val.relacionamento.cd_papel_pai
                            }
                        });
                }
            });

        return relacionamentos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarRelacSelecionados() {
    try {
        var storeRelac = [];
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        if (hasValue(gridRelacionamentos) && hasValue(gridRelacionamentos.store.objectStore.data)) {
            storeRelac = gridRelacionamentos.store.objectStore.data;
            if (storeRelac.length > 0) {
                storeRelac = jQuery.grep(storeRelac, function (value) {
                    return value.ehSelecionado == true;
                });
            }
        }
        return storeRelac;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//fim crud Relac

//Metodos Limpar pessoa
function limparGridOutrosEnderecos() {
    try {
        var gridEnderecos = dijit.byId("gridEnderecos");
        if (hasValue(gridEnderecos))
            gridEnderecos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGridOutrosContatos() {
    try {
        var gridContatos = dijit.byId("gridContatos");
        if (hasValue(gridContatos))
            gridContatos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGridRelacionamentos() {
    try {
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        if (hasValue(gridRelacionamentos))
            gridRelacionamentos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadPessoaFK() {
    try {
        dijit.byId("tagComplemento").set("open", false);
        $("#foto").attr({
            src: "",
            title: ""
        })
        dijit.byId("idPessoaAtiva").set("checked", true);
        if (dijit.byId("naturezaPessoa").value == PESSOAFISICA) {
            limparPessoaFisica();

        } else {
            limparPessoaJuridica();

        }
        limparEnderecoPrincipal();
        dijit.byId("panelEndereco").set("open", false);
        limparContatosPricipais()
        limparGridOutrosEnderecos();
        limparGridOutrosContatos();
        limparGridRelacionamentos();
        limparPhoto();
        sugereDataCorrente();
        configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodo que valida se o formulario esta preenchido corretamente.
function validateCadPessoaFK(windowUtils) {
    try {
        var validado = true;
        if (!hasValue(dojo.byId("email").value))
            dojo.byId("email").value = " ";
        else
            dojo.byId("email").value = dojo.byId("email").value.trim();
        if (!dijit.byId("email").validate()) {
            dijit.byId("panelEndereco").set("open", true);
            validado = false;
        }


        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        if (Permissoes != null && Permissoes != "" && possuiPermissao('pesE', Permissoes)) {

            if (hasValue(dijit.byId("tipoEndereco").get("value")) || hasValue(dijit.byId("logradouro").get("value")) || hasValue(dijit.byId("estado").get("value")) ||
                               hasValue(dijit.byId("cidade").get("value")) || hasValue(dojo.byId("cep").value) || hasValue(dojo.byId("bairro").value) ||
                               hasValue(dojo.byId("rua").value) || hasValue(dojo.byId("numero").value) || hasValue(dojo.byId("complemento").value)) {
                if (!validateFormEnderecoPrincipal(windowUtils))
                    validado = false;
            }
            if (!validateFormPessoa()) {
                validado = false;
            }
        } else {
            if (!validateFormEnderecoPrincipal(windowUtils))
                validado = false;
            if (!validateFormPessoa())
                validado = false;
        }
        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validateFormPessoa() {
    try {
        var retorno = true;
        if (!dijit.byId("formNatureza").validate()) {
            dijit.byId("panelPessoa").set("open", true);
            retorno = false;
        }

        if (dijit.byId("naturezaPessoa").value == PESSOAFISICA) {
            if ((dojo.byId("cdPessoaCpf").value == 0 || dojo.byId("cdPessoaCpf").value == "") && dijit.byId("formCpf").validate() == false) {
                dijit.byId("panelPessoa").set("open", true);
                retorno = false;
            }
            if (!dijit.byId("formSexo").validate()) {
                dijit.byId("panelPessoa").set("open", true);
                retorno = false;
            }
        } else {
            if (!dijit.byId("formCnpj").validate()) {
                dijit.byId("panelPessoa").set("open", true);
                retorno = false;
            }
        }

        if (!dijit.byId("formNomPessoa").validate()) {
            dijit.byId("panelPessoa").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("formDtaCadastramento").validate()) {
            dijit.byId("panelPessoa").set("open", true);
            retorno = false;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validateFormEnderecoPrincipal(windowUtils) {
    try {
        var retorno = true;
        if (!dijit.byId("tipoEndereco").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("tipoEndereco"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("rua").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("rua"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("logradouro").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("logradouro"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("bairro").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("bairro"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("estado").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("estado"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        if (!dijit.byId("cidade").validate()) {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("cidade"));
            dijit.byId("panelEndereco").set("open", true);
            retorno = false;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validateFormPessoaFisica() {
    try {
        if (!dijit.byId("formPessoaFisica").validate()) {
            dijit.byId("panelPessoaFisica").set("open", true);
            return false;
        }
        return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Metodo que retonra a pessoa selecionada na FK de pesquisa
//Retorno FKpessoa
function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPessoaSelec.itensSelecionados;
            if (dojo.byId("setValuePesquisaPessoaFK").value == USARCPF) {
                dojo.byId("cpf").value = "";
                dijit.byId("cpf").value = "";
                dojo.byId("cpf").oldValue = "";
                $("#cdPessoaCpf").val(value[0].cd_pessoa);
                $("#nomPessoaCpf").val(value[0].no_pessoa);
                dijit.byId("pesEndResp").set("disabled", false);
            }
            if (dojo.byId("setValuePesquisaPessoaFK").value == CPFRELAC) {
                $("#codPessoaRelac").val(value[0].cd_pessoa);
                $("#noPessoaRelac").val(value[0].no_pessoa);
                $("#inputRelac").val(value[0].nm_cpf_cgc);
                dijit.byId("inputRelac").set("disabled", true);
                dijit.byId("sexoRelac").set("value", value[0].nm_sexo);
                dijit.byId("sexoRelac").set("disabled", true);
                dijit.byId('noPessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').set("value",value[0].telefone);
            //    dijit.byId('emailRelac').attr("disabled", true);
            //    dijit.byId('emailRelac').set("value", value[0].email);
            }
            apresentaMensagem(dojo.byId("descApresMsg").value, null);
            dijit.byId("proPessoa").hide();
        }
        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoaCPFRELAC() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPessoaSelec.itensSelecionados;
            if (dojo.byId("setValuePesquisaPessoaFK").value == USARCPF) {
                dojo.byId("cpf").value = "";
                dijit.byId("cpf").value = "";
                dojo.byId("cpf").oldValue = "";
                $("#cdPessoaCpf").val(value[0].cd_pessoa);
                $("#nomPessoaCpf").val(value[0].no_pessoa);
                dijit.byId("pesEndResp").set("disabled", false);
            }
            if (dojo.byId("setValuePesquisaPessoaFK").value == CPFRELAC) {
                $("#codPessoaRelac").val(value[0].cd_pessoa);
                $("#noPessoaRelac").val(value[0].no_pessoa);
                $("#inputRelac").val(value[0].nm_cpf_cgc);
                dijit.byId("inputRelac").set("disabled", true);
                dijit.byId("sexoRelac").set("value", value[0].nm_sexo);
                dijit.byId("sexoRelac").set("disabled", true);
                dijit.byId('noPessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').set("value", value[0].telefone);
            //    dijit.byId('emailRelac').attr("disabled", true);
            //    dijit.byId('emailRelac').set("value", value[0].email);
            }
            apresentaMensagem(dojo.byId("descApresMsg").value, null);
            dijit.byId("proPessoa").hide();
        }
        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoaCPF() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPessoaSelec.itensSelecionados;
            if (dojo.byId("setValuePesquisaPessoaFK").value == USARCPF) {
//                dojo.byId("cpf").value = "";
//                dijit.byId("cpf").value = "";
//                dojo.byId("cpf").oldValue = "";
                $("#cdPessoaCpf").val(value[0].cd_pessoa);
                $("#nomPessoaCpf").val(value[0].no_pessoa);
                dijit.byId("pesEndResp").set("disabled", false);
            }
            if (dojo.byId("setValuePesquisaPessoaFK").value == CPFRELAC) {
                $("#codPessoaRelac").val(value[0].cd_pessoa);
                $("#noPessoaRelac").val(value[0].no_pessoa);
                $("#inputRelac").val(value[0].nm_cpf_cgc);
                dijit.byId("inputRelac").set("disabled", true);
                dijit.byId("sexoRelac").set("value", value[0].nm_sexo);
                dijit.byId("sexoRelac").set("disabled", true);
                dijit.byId('noPessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').attr("disabled", true);
                dijit.byId('telefonePessoaRelac').set("value", value[0].telefone);
            //    dijit.byId('emailRelac').attr("disabled", true);
            //    dijit.byId('emailRelac').set("value", value[0].email);
            }
            apresentaMensagem(dojo.byId("descApresMsg").value, null);
            dijit.byId("proPessoa").hide();
        }
        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showFoto(foto) {
    try {
        var excluirFoto = dijit.byId('excluirFoto');
        if (hasValue(foto) && hasValue(foto.ext_img_pessoa)) {
            $("#foto").attr({
                src: Endereco() + "/api/escola/GetPhoto?id=" + foto.cd_pessoa,
                title: "Imagem"
            }).css("max-height", "140px").css("max-width", "118px").css("display", "block");
            excluirFoto.setAttribute('disabled', 0);
        }
        else
            excluirFoto.setAttribute('disabled', 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFoto(urlFoto) {
    try {
        var excluirFoto = dijit.byId('excluirFoto');
        if (hasValue(foto) && hasValue(urlFoto)) {
            $("#foto").attr({
                src: Endereco() + "/api/escola/GetPhoto?nome=" + urlFoto,
                title: "Imagem"
            }).css("max-height", "140px").css("max-width", "118px").css("display", "block");
            
            excluirFoto.setAttribute('disabled', 0);
        }
        else
            excluirFoto.setAttribute('disabled', 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPhoto() {
    try {
        dojo.byId("setValueExt_foto_Pessoa").value = "";
        $("#foto").attr({
            src: "",
            title: ""
        }).css("display", "none");
        if (hasValue(dijit.byId("uploader")))
            dijit.byId("uploader")._files = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarImagemHelp(endereco, id) {
    require([
       "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                if (hasValue(endereco)) {
                    $(id).attr({
                        src: endereco
                    });
                    $(id)[0].src = endereco;
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

// Persistência da Atividade
function incluirAtividade() {
    try {
        if (!dijit.byId("formAtividade").validate())
            return false;
        apresentaMensagem('apresentadorMensagemAtividade', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.post(Endereco() + "/api/localidade/postAtividade", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_atividade: dom.byId("cd_atividade").value,
                    no_atividade: dom.byId("no_atividade").value,
                    cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                    id_natureza_atividade: dojo.byId("codNaturezaAtividade").value,
                    id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        apresentaMensagem(dojo.byId("descApresMsg").value, data);
                        var dados = data.retorno;
                        dojo.byId("cdAtividade").value = dados.cd_atividade;
                        dojo.byId("atividadePrincipal").value = dados.no_atividade;
                        dijit.byId('limparAtividade').set("disabled", false);
                        dijit.byId("formularioAtividade").hide();
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAtividade', data);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAtividade', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDadosPessoaFisica(date, apresentadorMensagem) {
    try {
        var outrosEnderecos = null;
        var outrosContatos = null;
        var descFoto = null;
        var relacionamentos = null;
        if (hasValue(dijit.byId("gridEnderecos")) && hasValue(dijit.byId("gridEnderecos").store.objectStore) && hasValue(dijit.byId("gridEnderecos").store.objectStore.data))
            outrosEnderecos = dijit.byId("gridEnderecos").store.objectStore.data;

        if (hasValue(dijit.byId("gridContatos")) && hasValue(dijit.byId("gridContatos").store.objectStore) && hasValue(dijit.byId("gridContatos").store.objectStore.data))

            outrosContatos = dijit.byId("gridContatos").store.objectStore.data;
        if (hasValue(dijit.byId("uploader")) && hasValue(dijit.byId("uploader")._files) && hasValue(dijit.byId("uploader")._files[0]))
            descFoto = getNameFoto(dijit.byId("uploader")._files[0]);

        if (hasValue(dijit.byId("gridRelacionamentos")))
            relacionamentos = montarRelacionamentosCompleto();

        var dataNascimento = hasValue(dojo.byId("dtaNasci").value) ? dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dataCadastro = hasValue(dojo.byId("horaCadastro").value) && hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value + " " + dojo.byId("horaCadastro").value, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }) : (hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null);
        if (date.compare(dataNascimento, dataCadastro) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataNascFinal);
            apresentaMensagem(apresentadorMensagem, mensagensWeb);
            return false;
        }

        var dadosRetorno = {
            pessoaFisica: {
                cd_pessoa: dojo.byId("cd_pessoa").value,
                no_Pessoa: $.trim(dojo.byId("nomPessoa").value.replace(/ {2,}/g, ' ')),
                dc_reduzido_pessoa: dojo.byId("nomFantasia").value,
                cd_atividade_principal: hasValue(dojo.byId('cdAtividade').value) && dojo.byId('cdAtividade').value > 0 ? dojo.byId('cdAtividade').value : null,
                dt_cadastramento: hasValue(dojo.byId("horaCadastro").value) && hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value + " " + dojo.byId("horaCadastro").value, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }) : (hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null),
                ext_img_pessoa: dojo.byId("setValueExt_foto_Pessoa").value,
                cd_estado_civil: hasValue(dijit.byId("estadoCivil").get("value")) ? dijit.byId("estadoCivil").get("value") : null,
                cd_loc_nascimento: hasValue(dojo.byId('codLocalNascimento').value) ? dojo.byId('codLocalNascimento').value : null,
                cd_loc_nacionalidade: hasValue(dijit.byId("nacionalidade").get("value")) ? dijit.byId("nacionalidade").get("value") : null,
                cd_tratamento: hasValue(dijit.byId("tratamento").get("value")) ? dijit.byId("tratamento").get("value") : null,
                cd_pessoa_cpf: hasValue(dojo.byId('cdPessoaCpf').value) && dojo.byId('cdPessoaCpf').value > 0 ? dojo.byId('cdPessoaCpf').value : null,
                nm_sexo: hasValue(dijit.byId("sexo").get("value")) ? dijit.byId("sexo").get("value") : 0,
                dt_nascimento: hasValue(dojo.byId("dtaNasci").value) ? dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                dt_casamento: hasValue(dojo.byId("dtaCasamento").value) ? dojo.date.locale.parse(dojo.byId("dtaCasamento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                nm_cpf: hasValue(dojo.byId("cpf").value) ? dojo.byId("cpf").value : null,
                nm_doc_identidade: hasValue(dojo.byId("nroRg").value) ? dojo.byId("nroRg").value : "",
                dc_carteira_trabalho: hasValue(dojo.byId("carteiraTrabalho").value) ? dojo.byId("carteiraTrabalho").value : "",
                dc_carteira_motorista: hasValue(dojo.byId("carteiraMotorista").value) ? dojo.byId("carteiraMotorista").value : "",
                dc_num_titulo_eleitor: hasValue(dojo.byId("tituloEleitor").value) ? dojo.byId("tituloEleitor").value : "",
                dc_num_insc_inss: hasValue(dojo.byId("nroInss").value) ? dojo.byId("nroInss").value : "",
                cd_orgao_expedidor: hasValue(dijit.byId("orgExp").get("value")) ? dijit.byId("orgExp").get("value") : null,
                cd_estado_expedidor: hasValue(dijit.byId("expRG").get("value")) ? dijit.byId("expRG").get("value") : null,
                dt_emis_expedidor: hasValue(dojo.byId("dtaEmiRg").value) ? dojo.date.locale.parse(dojo.byId("dtaEmiRg").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                txt_obs_pessoa: dojo.byId('descObs').value
            },
            cd_operadora: dijit.byId('operadora').value,
            telefone: dojo.byId("telefone").value,
            site: dojo.byId("site").value,
            email: dojo.byId("email").value,
            celular: dojo.byId("celular").value,
            endereco: {
                cd_endereco: dojo.byId("cdEndereco").value,
                cd_loc_cidade: hasValue(dijit.byId("cidade").get("value")) ? dijit.byId("cidade").get("value") : 0,
                cd_loc_estado: hasValue(dijit.byId("estado").get("value")) ? dijit.byId("estado").get("value") : 0,
                cd_loc_pais: 1,
                cd_tipo_endereco: hasValue(dijit.byId("tipoEndereco").get("value")) ? dijit.byId("tipoEndereco").get("value") : 0,
                cd_operadora: hasValue(dijit.byId("operadora").get("value")) ? dijit.byId("operadora").get("value") : 0,
                cd_tipo_logradouro: hasValue(dijit.byId("logradouro").get("value")) ? dijit.byId("logradouro").get('value') : null,
                cd_loc_bairro: hasValue(dijit.byId('bairro').get("value")) ? dijit.byId('bairro').get("value") : null,
                cd_loc_logradouro: hasValue(dojo.byId('codRua').value) ? dojo.byId('codRua').value : null,
                dc_compl_endereco: hasValue(dijit.byId("complemento").get("value")) ? dijit.byId("complemento").get("value") : '',
                num_cep: hasValue(dojo.byId("cep").value) ? dojo.byId("cep").value : '',
                dc_num_endereco: hasValue(dojo.byId("numero").value) ? dojo.byId("numero").value : ''
            },
            enderecos: outrosEnderecos,
            telefones: outrosContatos,
            descFoto: descFoto,
            relacionamentosUI: relacionamentos
        }

        return dadosRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDadosPessoaJuridica(dom, domAttr) {
    try {
        var outrosEnderecos = null;
        var outrosContatos = null;
        var relacionamentos = null;
        var descFoto = null;

        if (hasValue(dijit.byId("gridRelacionamentos")))
            relacionamentos = montarRelacionamentos();
        if (hasValue(dijit.byId("gridEnderecos")) && hasValue(dijit.byId("gridEnderecos").store.objectStore) && hasValue(dijit.byId("gridEnderecos").store.objectStore.data))
            outrosEnderecos = dijit.byId("gridEnderecos").store.objectStore.data;
        if (hasValue(dijit.byId("gridContatos")) && hasValue(dijit.byId("gridContatos").store.objectStore) && hasValue(dijit.byId("gridContatos").store.objectStore.data))
            outrosContatos = dijit.byId("gridContatos").store.objectStore.data;
        if (hasValue(dijit.byId("uploader")) && hasValue(dijit.byId("uploader")._files) && hasValue(dijit.byId("uploader")._files[0]))
            descFoto = getNameFoto(dijit.byId("uploader")._files[0]);

        var atividade = hasValue(dojo.byId("cdAtividade").value) & dojo.byId("cdAtividade").value > 0 ? dojo.byId("cdAtividade").value : null;

        var dadosRetorno = {
            pessoaJuridica: {
                cd_pessoa: dom.byId("cd_pessoa").value,
                no_Pessoa: dom.byId("nomPessoa").value,
                dc_reduzido_pessoa: dom.byId("nomFantasia").value,
                cd_atividade_principal: atividade,
                dt_cadastramento: hasValue(dojo.byId("horaCadastro").value) && hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value + " " + dojo.byId("horaCadastro").value, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }) : (hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null),
                dc_num_cgc: dom.byId("cnpj").value,
                ext_img_pessoa: dojo.byId("setValueExt_foto_Pessoa").value,
                cd_tipo_sociedade: dijit.byId('tipoSociedade').value,
                dc_num_insc_estadual: dom.byId("dcNumInscEstadual").value,
                dc_num_insc_municipal: dom.byId("dcNumInscMunicipal").value,
                dc_num_cnpj_cnab: dom.byId("dcNumCnpjCnab").value,
                dt_registro_junta_comercial: hasValue(dojo.byId("dtRegistroJuntaComercial").value) ? dojo.date.locale.parse(dojo.byId("dtRegistroJuntaComercial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                dt_baixa: hasValue(dojo.byId("dtaBaixa").value) ? dojo.date.locale.parse(dojo.byId("dtaBaixa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                nm_natureza_pessoa: hasValue(dijit.byId("naturezaPessoa").value) ? dijit.byId("naturezaPessoa").value : 0,
                id_exportado: 0,
                txt_obs_pessoa: dom.byId('descObs').value

            },
            cd_operadora: dijit.byId('operadora').value,
            telefone: dom.byId("telefone").value,
            site: dom.byId("site").value,
            email: dom.byId("email").value,
            celular: dom.byId("celular").value,
            endereco: {
                cd_loc_cidade: hasValue(dijit.byId("cidade").value) ? dijit.byId("cidade").value : 0,
                cd_loc_estado: hasValue(dijit.byId("estado").get("value")) ? dijit.byId("estado").get("value") : 0,
                cd_loc_pais: 1,
                cd_tipo_endereco: hasValue(dijit.byId("tipoEndereco").get("value")) ? dijit.byId("tipoEndereco").get("value") : 0,
                cd_operadora: hasValue(dijit.byId("operadora").get("value")) ? dijit.byId("operadora").get("value") : 0,
                cd_tipo_logradouro: hasValue(dijit.byId("logradouro").get("value")) ? dijit.byId("logradouro").get('value') : null,
                cd_loc_bairro: hasValue(dijit.byId('bairro').value) ? dijit.byId('bairro').value : null,
                cd_loc_logradouro: hasValue(dojo.byId('codRua').value) ? dojo.byId('codRua').value : null,
                dc_compl_endereco: hasValue(dijit.byId("complemento").get("value")) ? dijit.byId("complemento").get("value") : '',
                num_cep: hasValue(dojo.byId("cep").value) ? dojo.byId("cep").value : '',
                dc_num_endereco: hasValue(dojo.byId("numero").value) ? dojo.byId("numero").value : ''
            },
            enderecos: outrosEnderecos,
            telefones: outrosContatos,
            descFoto: descFoto,
            relacionamentosUI: relacionamentos
        };

        return dadosRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function observeTitlePaneOpenUpdate() {
    try {
        if (dijit.byId("tagComplemento").open) {
            if (dijit.byId("panelOutrosEnderecos").open)
                dijit.byId("gridEnderecos").update();
            if (dijit.byId("panelOutrosContatos").open)
                dijit.byId("gridContatos").update();
            if (dijit.byId("panelPessoaRelacionadas").open)
                dijit.byId("gridRelacionamentos").update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFK() {
    try {
        dojo.byId("setValuePesquisaPessoaFK").value = USARCPF;
        limparPesquisaPessoaFK();
        dijit.byId("tipoPessoaFK").set("value", 1);
        dijit.byId("tipoPessoaFK").set("disabled", true);
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        pesquisaPessoaCadFK();
        dijit.byId("proPessoa").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFKCPF() {
    try {
        dojo.byId("setValuePesquisaPessoaFK").value = USARCPF;
        dojo.byId("idOrigemPessoaFK").value = USARCPF;
        limparPesquisaPessoaFK();
        dijit.byId("tipoPessoaFK").set("value", 1);
        dijit.byId("tipoPessoaFK").set("disabled", true);
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        pesquisaPessoaCadFK();
        dijit.byId("proPessoa").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFKRelac() {
    try {
        dojo.byId("setValuePesquisaPessoaFK").value = CPFRELAC;
        dojo.byId("idOrigemPessoaFK").value = CPFRELAC;
        var tipoPessoa = dijit.byId('naturezaPessoaRelac').value;
        limparPesquisaPessoaFK()
        dijit.byId("tipoPessoaFK").set("value", tipoPessoa);
        dijit.byId("tipoPessoaFK").set("disabled", true);
        apresentaMensagem('apresMsgNewRelac', null);
        var papel = dojo.byId("codPapelRelac").value;
        pesquisaPessoaCadFKPessoaRelac(papel);
        dijit.byId("proPessoa").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaCadFKPessoaRelac(papel) {
    require([
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojox/data/JsonRestStore",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready",
        "dojo/domReady!",
        "dojo/parser"],
    function (xhr, _registry, EnhancedGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            var myStore = Cache(
                 JsonRest({
                     target: Endereco() + "/api/aluno/getPessoaSearchEscolaWithCPFCNPJ?nome=" + dojo.byId("_nomePessoaFK").value +
                                   "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                   "&cnpjCpf=" + dojo.byId("CnpjCpf").value + "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=" + papel,
                     handleAs: "json",
                     headers: { "Accept": "application/json", "Authorization": Token() }
                 }), Memory({}));

            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaPessoa");
            grid.setStore(dataStore);
            grid.itensSelecionados = [];
        })
    });
}

function pesquisaPessoaCadFK() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaResponsavelCPFSearchEscola?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value + "&sexo=" + dijit.byId("sexoPessoaFK").value,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function montarPesquisaPessoaFKPorTipo() {
    try {
        tipoPesq = dojo.byId("setValuePesquisaPessoaFK").value;
        apresentaMensagem("apresentadorMensagemProPessoa", null);
        var papel = dojo.byId("codPapelRelac").value;
        if (tipoPesq == USARCPF)
            pesquisaPessoaCadFK();
        if (tipoPesq == CPFRELAC)
            pesquisaPessoaCadFKPessoaRelac(papel);
        dijit.byId("proPessoa").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    try {
        dojo.xhr.post({
            url: Endereco() + "/util/PostDataHoraCorrente",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson()
        }).then(function (data) {
            if (data.indexOf("<!DOCTYPE html>") < 0) {
                var dataCorrente = jQuery.parseJSON(data).retorno;
                var dataSugerida = dataCorrente.dataPortugues.split(" ");
                if (dataSugerida.length > 0) {
                    var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    dijit.byId('dtaCadastro').set("value", date);
                    var hora = dataSugerida[1].split(":");
                    if (hasValue(hora) && hora[0].length < 2)
                        hora[0] = "0" + hora[0];
                    var horasTimeSpin = "T" + hora[0] + ":" + hora[1] + ":" + hora[2];
                    dijit.byId('horaCadastro').set("value", horasTimeSpin);
                }
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemPessoa', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPesquisaCidadeFKPessoaRelacionada() {
    try {
        clearForm('pesquisaCidadeFK');
        //pesquisaCidadeFK();
        limparFiltrosCidaddeFK();
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = PESSOARELACIONADA;
        dijit.byId("paisFk").set("value", 1);
        pesquisaCidadeFK(true);
        dijit.byId("dialogConsultaCidade").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarCidadePessoaRelacionada() {
    try {
        var valido = true;
        var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
        if (!hasValue(gridPesquisaCidade.itensSelecionados) || gridPesquisaCidade.itensSelecionados.length <= 0 || gridPesquisaCidade.itensSelecionados.length > 1) {
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("codLocalNascimento").value = gridPesquisaCidade.itensSelecionados[0].cd_cidade;
            dijit.byId("descLocalNascimento").set("value", gridPesquisaCidade.itensSelecionados[0].no_cidade);
        }
        if (!valido)
            return false;
        dijit.byId("dialogConsultaCidade").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Reestruturação do cadastro de endereço.

function limparFiltrosLogradouroFKEnredecoPessoa() {
    limpaCidadeLogradouroFK();
    var compEstadoLogFK = dijit.byId("estadFKLogradouro");
    compEstadoLogFK._onChangeActive = false;
    compEstadoLogFK.reset();
    compEstadoLogFK._onChangeActive = true;
    dijit.byId("outrosLogradouros").reset();
    dojo.byId("descricaoLogradouroFK").value = "";
    showP('imgIntDescricaoPesqLogradouro', false);
    //showP('trNumeroEndereco', false);
    var gridLogradouroFK = dijit.byId("gridLogradouroFK");
    if (hasValue(gridLogradouroFK) && hasValue(gridLogradouroFK.itensSelecionados))
        gridLogradouroFK.itensSelecionados = [];
    gridLogradouroFK.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory(null) }));
    gridLogradouroFK.update();

}

function abrirPesquisaLogradouroFK(data) {
    try {
        limparFiltrosLogradouroFKEnredecoPessoa();
        dojo.byId('setValuePesquisaLogradouroFK').value = PESCADENDERECOPESSOA;
        var compEstado = dijit.byId("estado");
        var compCidade = dijit.byId("cidade");
        var compBairro = dijit.byId("bairro");
        var compNumero = dijit.byId("numeroEndereco");
        var compEstadoLogFK = dijit.byId("estadFKLogradouro");
        var cd_estado = 0;
        var cd_cidade = 0;
        var cd_bairro = 0;
        var nome_cidade = "";
        if (data != null) {
            cd_estado = data.cd_loc_estado;
            cd_cidade = data.cd_loc_cidade;
            nome_cidade = data.noLocCidade;
            //cd_bairro = data.cd_loc_bairro;
        } else {
            cd_estado = compEstado.value;
            cd_cidade = compCidade.value;
            //cd_bairro = compBairro.value;
            nome_cidade = compCidade.displayedValue;
        }
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = 1;

        if (cd_estado > 0) {
            compEstadoLogFK._onChangeActive = false;
            compEstadoLogFK.set("value", cd_estado);
            compEstadoLogFK._onChangeActive = true;
        }
        compNumero.reset();
        //compBairro.reset();
        if (cd_cidade > 0) {
            dojo.byId("cd_cidade_pesq_logradouroFK").value = cd_cidade;
            dijit.byId("pesNomCidadeLogradouroFK").set("value", nome_cidade);
            dojo.byId("pesNomCidadeLogradouroFK").value = nome_cidade;
            dijit.byId("limparCidadeLogradouroPesFK").set("disabled", false);
            carregarBairroPorCidadeFK(cd_cidade, cd_bairro, function () {
                pesquisarLogradourofk(true);
                dijit.byId("proLogradouroFK").show();
                dijit.byId('gridLogradouroFK').update();
            });
            return false;
        }
        pesquisarLogradourofk(true);
        dijit.byId("proLogradouroFK").show();
        dijit.byId('gridLogradouroFK').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPesquisaLogradouroFKOutrosEnd(data) {
    try {
        limparFiltrosLogradouroFKEnredecoPessoa();
        dojo.byId('setValuePesquisaLogradouroFK').value = PESCADENDERECOPESSOA;
        var compEstadoLogFK = dijit.byId("estadFKLogradouro");
        compEstadoLogFK._onChangeActive = false;
        compEstadoLogFK.reset();
        var compEstado = dijit.byId("estadoOutrosEnd");
        var compCidade = dijit.byId("cidadeOutrosEnd");
        var compBairro = dijit.byId("bairroOutrosEnd");
        var cd_estado = 0;
        var cd_cidade = 0;
        var cd_bairro = 0;
        var nome_cidade = "";
        if (data != null) {
            cd_estado = data.cd_loc_estado;
            cd_cidade = data.cd_loc_cidade;
            nome_cidade = data.noLocCidade;
            cd_bairro = data.cd_loc_bairro;
        } else {
            cd_estado = compEstado.value;
            cd_cidade = compCidade.value;
            cd_bairro = compBairro.value;
            nome_cidade = compCidade.displayedValue;
        }
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = 1;
        if (cd_estado > 0) {
            var compEstadoLogFK = dijit.byId("estadFKLogradouro");
            compEstadoLogFK._onChangeActive = false;
            compEstadoLogFK.set("value", cd_estado);
            compEstadoLogFK._onChangeActive = true;
        }
        if (cd_cidade > 0) {
            dojo.byId("cd_cidade_pesq_logradouroFK").value = cd_cidade;
            dijit.byId("pesNomCidadeLogradouroFK").set("value", nome_cidade);
            dijit.byId("limparCidadeLogradouroPesFK").set("disabled", false);
            carregarBairroPorCidadeFK(cd_cidade, cd_bairro, function () {
                pesquisarLogradourofk(true);
                dijit.byId("proLogradouroFK").show();
                dijit.byId('gridLogradouroFK').update();
            });
            return false;
        }
        pesquisarLogradourofk(true);
        dijit.byId("proLogradouroFK").show();
        dijit.byId('gridLogradouroFK').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarLogradouroFK() {
    try{
        var valido = true;
        var gridLogradouroFK = dijit.byId("gridLogradouroFK");
        if (!hasValue(gridLogradouroFK.itensSelecionados) || gridLogradouroFK.itensSelecionados.length <= 0 || gridLogradouroFK.itensSelecionados.length > 1) {
            if (gridLogradouroFK.itensSelecionados != null && gridLogradouroFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridLogradouroFK.itensSelecionados != null && gridLogradouroFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var itemSelecionado = gridLogradouroFK.itensSelecionados[0];
            if (!dijit.byId("DialogEndereco").open) {

                if (!hasValue(itemSelecionado.cd_localidade)) {
                    var data = {
                        tipoLogradouro: itemSelecionado.no_tipo_logradouro,
                        noLocRua: itemSelecionado.no_localidade,
                        noLocCidade: itemSelecionado.no_localidade_cidade,
                        noLocBairro: itemSelecionado.no_localidade_bairro,
                        noLocEstado: itemSelecionado.no_localidade_estado,
                        descTipoLogradouro : itemSelecionado.no_tipo_logradouro,
                        num_cep: itemSelecionado.dc_num_cep
                    };
                    pesquisarEnderecoOrCadastrar(data, ENDERECOPRINCIPAL, 'apresentadorMensagemPessoa', false);
                }
                else {
                    if (itemSelecionado.cd_localidade == 0 && itemSelecionado.nm_cep == "")
                        throw new Exception("Registro inconsistente. Endereço : " + JSON.stringify(itemSelecionado));
                    carregarEndereoByLogradouroOrCep(itemSelecionado.cd_localidade, "");
                    dojo.byId("codRua").value = itemSelecionado.cd_localidade;
                    dijit.byId("rua").set("value", itemSelecionado.no_localidade);
                }
            } else {
                if (!hasValue(itemSelecionado.cd_localidade)) {
                    var data = {
                        tipoLogradouro: itemSelecionado.no_tipo_logradouro,
                        noLocRua: itemSelecionado.no_localidade,
                        noLocCidade: itemSelecionado.no_localidade_cidade,
                        noLocBairro: itemSelecionado.no_localidade_bairro,
                        noLocEstado: itemSelecionado.no_localidade_estado,
                        descTipoLogradouro: itemSelecionado.no_tipo_logradouro,
                        num_cep: itemSelecionado.dc_num_cep
                    };
                    pesquisarEnderecoOrCadastrar(data, OUTROSENDERECOS, 'apresentadorMsgOutosEnderecos', false);
                }
                else {
                    if (itemSelecionado.cd_localidade == 0 && itemSelecionado.nm_cep == "")
                        throw new Exception("Registro inconsistente. Endereço : " + JSON.stringify(itemSelecionado));
                    carregarEndereoByLogradouroOrCepOutrosEnderecos(itemSelecionado.cd_localidade, "");
                    dojo.byId("codRuaOutrosEnd").value = itemSelecionado.cd_localidade;
                    dijit.byId("ruaOutrosEnd").set("value", itemSelecionado.no_localidade);
                }
            }
        }
        if (!valido)
            return false;
        dijit.byId("proLogradouroFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarEndereoByLogradouroOrCep(cd_logradouro, nm_cep) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getEnderecoByCdLogradouro?cd_logradouro=" + cd_logradouro + "&nm_cep=" + nm_cep,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                data = jQuery.parseJSON(data).retorno;
                configurarLayoutEndereco(LAYOUTPESQUISALOGRADOURO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
                setValuesEndereco(data, "estado", "cidade", "bairro", "rua", "codRua", "cep", "logradouro");
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemPessoa", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarEndereoByLogradouroOrCepOutrosEnderecos(cd_logradouro, nm_cep) {
    try{
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getEnderecoByCdLogradouro?cd_logradouro=" + cd_logradouro + "&nm_cep=" + nm_cep,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                data = jQuery.parseJSON(data).retorno;
                configurarLayoutEndereco(LAYOUTPESQUISALOGRADOURO, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd",
                                        "ruaOutrosEnd", "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
                setValuesEndereco(data, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd", "ruaOutrosEnd", "codRuaOutrosEnd", "cepOutrosEnd", "tipoLogradouroOutrosEnd");
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemPessoa", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarRetornoEnderecoPrincipal(data, pesquisarWebServiceCep,nm_cep,idMsg) {
    try{
        var compCep = dijit.byId("cep");
        if (data != null) {
            configurarLayoutEndereco(LAYOUTPESQUISACEP, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
            setValuesEndereco(data, "estado", "cidade", "bairro", "rua","codRua", "cep", "logradouro");
        } else {
            caixaDialogo(DIALOGO_AVISO, msgInfoCEPNaoEncontrado);
            configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
        }
        compCep._onChangeActive = false;
        compCep.set("value", compCep.value);
        compCep._onChangeActive = true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarRetornoEnderecoOutrosEnderecos(data, pesquisarWebServiceCep, nm_cep, idMsg) {
    try{
        var compCep = dijit.byId("cepOutrosEnd");
        if (data != null) {
            configurarLayoutEndereco(LAYOUTPESQUISACEP, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd",
                                    "ruaOutrosEnd", "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
            setValuesEndereco(data, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd", "ruaOutrosEnd", "codRuaOutrosEnd", "cepOutrosEnd", "tipoLogradouroOutrosEnd");
        } else {
            caixaDialogo(DIALOGO_AVISO, msgInfoCEPNaoEncontrado);
            configurarLayoutEndereco(LAYOUTPADRAO, "estadoOutrosEnd", "cidadeOutrosEnd", "bairroOutrosEnd",
                                    "ruaOutrosEnd", "cepOutrosEnd", "btnCidadeDialog", "btnBairroOutrosEnd", "btnRuaOutrosEnd");
        }
        compCep._onChangeActive = false;
        compCep.set("value", compCep.value);
        compCep._onChangeActive = true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Cadastrar Cidade Modal
function cadastrarCidade() {
    try{
        var tipoCadastro = dojo.byId("setDefaultValueCEP").value;
        var idEstado = "", idCidade = "", idMsg = "";
        if (tipoCadastro == ENDERECOPRINCIPAL) {
            idEstado = "estado";
            idCidade = "cidade";
            idMsg = dojo.byId("descApresMsg").value;
        } else {
            idEstado = "estadoOutrosEnd";
            idCidade = "cidadeOutrosEnd";
            idMsg = "apresentadorMsgOutosEnderecos";
        }
        var compEstado = dijit.byId(idEstado);
   
        if (!dijit.byId("formCadCidadeDialog").validate()) {
            return false;
        }
        if (!hasValue(compEstado.value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCadCidadeDialog);
            apresentaMensagem('apresentadorMsgOutosEnderecos', mensagensWeb);
            return false;
        }
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/PostLocalidadePessoaCidade",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                no_localidade: dojo.byId("desCidadeDialog").value,
                cd_loc_relacionada: compEstado.value,
                nm_municipio: parseInt((dojo.byId("nm_municipioCidade").value).replace(".", ""))
            })
        }).then(function (data) {
            try{
                var dados = jQuery.parseJSON(data).retorno;
                if (hasValue(dados.localidades) || dados.localidades.length == 0) {
                    dijit.byId(idCidade).reset();
                    criarOuCarregarCompFiltering(idCidade, dados.localidades, "", dados.cd_localidade_cidade, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
                }
                apresentaMensagem(idMsg, data);
                dijit.byId("dialogCidade").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemCidade", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Cadastrar Bairro Modal
function cadastrarBairro() {
    try{
        var tipoCadastro = dojo.byId("setDefaultValueCEP").value;
        var idCidade = "", idbairro = "", idMsg = "";
        if (tipoCadastro == ENDERECOPRINCIPAL) {
            idCidade = "cidade";
            idBairro = "bairro";
            idMsg = dojo.byId("descApresMsg").value;
        } else {
            idCidade = "cidadeOutrosEnd";
            idBairro = "bairroOutrosEnd";
            idMsg = "apresentadorMsgOutosEnderecos";
        }
        var compCidade = dijit.byId(idCidade);

        if (!dijit.byId("formCadBairro").validate()) {
            return false;
        }

        if (!hasValue(compCidade.value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCadBairroDialog);
            apresentaMensagem('apresentadorMsgOutosEnderecos', mensagensWeb);
            return false;
        }
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/PostLocalidadePessoaBairro",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                cd_localidade: null,
                no_localidade: dojo.byId("desBairroDialog").value,
                cd_loc_relacionada: compCidade.value,
                nm_municipio: null
            })
        }).then(function (data) {
            try{
                var dados = jQuery.parseJSON(data).retorno;
                if (hasValue(dados.localidades) || dados.localidades.length == 0) {
                    criarOuCarregarCompFiltering(idBairro, dados.localidades, "", dados.cd_localidade_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
                    dijit.byId(idBairro).oldValue = dados.cd_localidade_bairro;
                }
                apresentaMensagem(idMsg, data);
                dijit.byId("dialogBairro").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemBairro", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Cadastro Logradouro 
function cadastrarRua() {
    try{
        var tipoCadastro = dojo.byId("setDefaultValueCEP").value;
        var idBairro = "", idRua = "", idCodRua = null, idMsg = "";
        if (tipoCadastro == ENDERECOPRINCIPAL) {
            idBairro = "bairro";
            idRua = "rua";
            idCodRua = "codRua";
            idMsg = dojo.byId("descApresMsg").value;
        } else {
            idBairro = "bairroOutrosEnd";
            idRua = "ruaOutrosEnd";
            idCodRua = "codRuaOutrosEnd";
            idMsg = "apresentadorMsgOutosEnderecos";
        }
        var compBairro = dijit.byId(idBairro);

        if (!dijit.byId("formCadRua").validate()) {
            return false;
        }
        if (!hasValue(compBairro.value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCadLogradouroDialog);
            apresentaMensagem('apresentadorMsgOutosEnderecos', mensagensWeb);
            return false;
        }
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/PostLocalidadePessoaLogradouro",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                cd_localidade: null,
                no_localidade: dojo.byId("desRuaDialog").value,
                cd_loc_relacionada: compBairro.value,
                nm_municipio: null
            })
        }).then(function (data) {
            try{
                var dados = jQuery.parseJSON(data).retorno;
                dojo.byId(idCodRua).value = dados.cd_localidade;
                dojo.byId(idRua).value = dados.no_localidade;
                apresentaMensagem(idMsg, data);
                dijit.byId("dialogRua").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemRua", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPesquisaAuxiliaresPessoaFK(tipoPessoa) {
    try{
    var natureza = dijit.byId('naturezaPessoa').get('value');
    natureza = hasValue(natureza) ? natureza : PESSOAFISICA;
    if (natureza == PESSOAFISICA)
        dijit.byId('cnaeFK').set('disabled', true);
    else
        dijit.byId('cnaeFK').set('disabled', false);
    clearFormAtividadeFk(natureza);
    dijit.byId("proAuxiliaresPessoaFK").show();
    dijit.byId("gridAuxiliaresPessoaFK").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}
