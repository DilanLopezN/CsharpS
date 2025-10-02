var TODOS = 0, PROSPECTS_ATENDIDOS = 1;
var COMPARATIVO_PROSPECTS_ATENDIDOS_PERIODOS = 4;
function montarMetodosRelatorioProspect() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, Memory, on, Button, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            try {
                sugereDataCorrente();
                loadPeriodo(Memory, registry, ItemFileReadStore);
                carregarFaixaEtaria(ready, Memory, FilteringSelect);

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioProspect");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId('gridPesquisaProfessor')))
                                montarGridPesquisaProfessor(true, function () {
                                    abrirProfessorFK();
                                    dijit.byId("pesquisarProfessorFK").on("click", function (event) {
                                        apresentaMensagem('apresentadorMensagem', null);
                                        pesquisarFuncionarioFK();
                                    });
                                });
                            else abrirProfessorFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesFuncionario");
                decreaseBtn(document.getElementById("pesFuncionario"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dojo.byId("cd_funcionario").value = 0;
                            dojo.byId("cd_pessoa_funcionario").value = 0;
                            dojo.byId("noFuncionario").value = "";
                            dijit.byId("limparPesFuncionario").set('disabled', true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                }, "limparPesFuncionario");
                decreaseBtn(document.getElementById("limparPesFuncionario"), '40px');
                montarTipoRelatorio("pesRelatorio", Memory, on);
                //montarMtNaoMatricula("mtNnaoMatricula", Memory);
                dijit.byId("pesProduto").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("pesProduto").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesRelatorio").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < 0)
                            dijit.byId("pesRelatorio").set("value", PROSPECTS_ATENDIDOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("mtNnaoMatricula").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("mtNnaoMatricula").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323076', '765px', '771px');
                        });
                }
                getComponentesPesquisaRelatorio();
                getMidiasProspect(xhr, ready, Memory, FilteringSelect);
                dijit.byId("compComoConheceu").on("change", function (cd_tipo) {
                    try {
                        if (!hasValue(cd_tipo) || cd_tipo < 0)
                            dijit.byId("compComoConheceu").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function getMidiasProspect(xhr, ready, Memory, FilteringSelect) {
    xhr.get({
        url: Endereco() + "/api/secretaria/getMidiasProspect",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var data = jQuery.parseJSON(data);
            if (data != null && data.length > 0)
                criarOuCarregarCompFiltering("compComoConheceu", data, "", null, ready, Memory, FilteringSelect, 'cd_midia', 'no_midia', MASCULINO);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagemMovto", error);
    });
}

function getComponentesPesquisaRelatorio() {
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getComponentesPesquisaRelatorioListagemProspect",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            showCarregando();
            if (hasValue(dados.retorno) && hasValue(dados.retorno.listaProdutos))
                criarOuCarregarCompFiltering("pesProduto", dados.retorno.listaProdutos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                             'cd_produto', 'no_produto', MASCULINO);
            if (hasValue(dados.retorno) && hasValue(dados.retorno.listaMotivosNaoMatricula))
                criarOuCarregarCompFiltering("mtNnaoMatricula", dados.retorno.listaMotivosNaoMatricula, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                            'cd_motivo_nao_matricula', 'dc_motivo_nao_matricula', MASCULINO);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function montarTipoRelatorio(nomElement, Memory, on) {

    var dados = [
        { name: "Prospects Atendidos", id: "1" },
        { name: "Prospects Atendidos por Motivo da Não Matrícula", id: "2" },
        { name: "Prospect Atendidos por Matrículas", id: "3" },
        { name: "Comparativo de Prospects Atendidos por Períodos", id: "4" }
    ]
    var statusStore = new Memory({
        data: dados
    });
    dijit.byId(nomElement).store = statusStore;
    dijit.byId(nomElement).set("value", PROSPECTS_ATENDIDOS);
    dijit.byId(nomElement).on("change", function (cdTipoRela) {
        if (isNaN(cdTipoRela))
            dijit.byId(nomElement).set('value', PROSPECTS_ATENDIDOS);
        else {
            if (cdTipoRela == COMPARATIVO_PROSPECTS_ATENDIDOS_PERIODOS) {
                
                dojo.byId("trCompDta").style.display = "";
                dijit.byId("paneDataInicial").set("title", "Períodos");
                riqueridDatas(true);
            } else {
                riqueridDatas(false);
                dijit.byId("paneDataInicial").set("title", "Período do Cadastro");
                dojo.byId("trCompDta").style.display = "none";
            }
        }
    });
};

function riqueridDatas(bool) {
    dijit.byId("dtInicialComp").set("required",bool);
    dijit.byId("dtFinalComp").set("required", bool);
}

function abrirProfessorFK() {
    try {
        limparPesquisaProfessorFK();
        pesquisarFuncionarioFK();
        dijit.byId("proProfessor").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarProfessorFK() {
    try {
        var valido = true;
        var gridPesquisaProfessor = dijit.byId("gridPesquisaProfessor");
        if (!hasValue(gridPesquisaProfessor.itensSelecionados) || gridPesquisaProfessor.itensSelecionados.length <= 0 || gridPesquisaProfessor.itensSelecionados.length > 1) {
            if (gridPesquisaProfessor.itensSelecionados == null || gridPesquisaProfessor.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaProfessor.itensSelecionados != null && gridPesquisaProfessor.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {
            dojo.byId("cd_funcionario").value = gridPesquisaProfessor.itensSelecionados[0].cd_funcionario;
            dojo.byId("cd_pessoa_funcionario").value = gridPesquisaProfessor.itensSelecionados[0].cd_pessoa_funcionario;
            dojo.byId("noFuncionario").value = gridPesquisaProfessor.itensSelecionados[0].dc_reduzido_pessoa;
        }
        if (!valido)
            return false;
        dijit.byId("limparPesFuncionario").set("disabled", false);
        dijit.byId("proProfessor").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function emitirRelatorio() {
    try {
        var cd_mtv_nao_matricula = hasValue(dijit.byId("mtNnaoMatricula").value) ? dijit.byId("mtNnaoMatricula").value : 0;
        var cd_funcionario = hasValue(dojo.byId("cd_pessoa_funcionario").value) ? dojo.byId("cd_pessoa_funcionario").value : 0;
        var cd_produto = hasValue(dijit.byId("pesProduto").value) ? dijit.byId("pesProduto").value : 0;
        var cd_produtos = dijit.byId("cbPeriodo").value;
        var cd_faixa_etaria = hasValue(dijit.byId("idFaixaEtaria").value) ? dijit.byId("idFaixaEtaria").value : 0;

        if (!dijit.byId("formPesquisaRelatorioProspect").validate())
            return false;
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/GetUrlRelatorioProspectAtendido?cd_mtv_nao_matricula=" + cd_mtv_nao_matricula + "&cd_funcionario=" + cd_funcionario + "&cd_produto=" + cd_produto +
                               "&dta_ini=" + dojo.byId('dtInicial').value + "&dta_fim=" + dojo.byId('dtFinal').value + "&tipo=" + dijit.byId("pesRelatorio").value +
                               "&dta_ini_comp=" + dojo.byId('dtInicialComp').value + "&dta_fim_comp=" + dojo.byId('dtFinalComp').value + "&cd_midia=" + dijit.byId("compComoConheceu").value +
                               "&cd_produtos=" + cd_produtos + "&cd_faixa_etaria=" + cd_faixa_etaria,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioProspectAtendido?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarFuncionarioFK() {
    try {
        //Caso tenha horários, verifica se existe horários marcados:
        var trHorarios = document.getElementById('trHorarios');

        if (trHorarios.style.display == 'none' || (trHorarios.style.display != 'none' && dijit.byId('cbHorarios').validate())) {
            var sexo = hasValue(dijit.byId("nm_sexo_prof").value) ? dijit.byId("nm_sexo_prof").value : 0;
            var status = hasValue(dijit.byId("statusProf").value) ? dijit.byId("statusProf").value : 0;
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/funcionario/GetFuncionarioSearch?nome=" + dojo.byId("nomeProf").value + "&apelido=" + dojo.byId("nomeRed").value + "&status=" + parseInt(status) +
                            "&cpf=" + dojo.byId("cpfProf").value + "&inicio=" + document.getElementById("inicioProf").checked + "&tipo=" + FUNCIONARIO + "&sexo=" + dijit.byId("nm_sexo_prof").value + "&cdAtividade=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_professor"
                    }), Memory({ idProperty: "cd_professor" }));
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var gridProfessor = dijit.byId("gridPesquisaProfessor");
                    gridProfessor.itensSelecionados = [];
                    gridProfessor.setStore(dataStore);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function loadPeriodo(Memory, registry, ItemFileReadStore) {
    try {
        var w = registry.byId("cbPeriodo");
        var stateStore = [
                   { name: "Manhã", id: "1" },
                   { name: "Tarde", id: "2" },
                   { name: "Noite", id: "3" }
        ];
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: stateStore
            }
        });
        w.setStore(store, []);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarFaixaEtaria(ready, Memory, FilteringSelect) {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/getModalidades?criterios=&cd_modalidade=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            criarOuCarregarCompFiltering("idFaixaEtaria", jQuery.parseJSON(dados).retorno, "", TODOS, ready, Memory, FilteringSelect, 'cd_modalidade', 'no_modalidade', MASCULINO);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}