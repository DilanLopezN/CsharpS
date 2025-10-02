
function montarMetodosRelatorioHistoricoAluno() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dijit/Tooltip",
    "dojo/on",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, Memory, on, Button, JsonRest, ObjectStore, Cache, Tooltip, on, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            try {
                showCarregando();

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioHistoricoAluno");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");

                decreaseBtn(document.getElementById("pesAluno"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPesq").value = 0;
                        dojo.byId("noAluno").value = "";
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                    }
                }, "limparAluno");
                decreaseBtn(document.getElementById("limparPesAluno"), '40px');

                loadProduto('cbHistorico', ItemFileReadStore);
                loadAvaliacao('cbAvaliacao', ItemFileReadStore);
                loadTitulos('cbTitulos', ItemFileReadStore);
                loadAvalEstagio('cbAvalEstagio', ItemFileReadStore);

                /* Coloca os checks marcados como Default */
                dijit.byId("ckEstagio").set("checked", true);
                dijit.byId("ckAtividade").set("checked", true);
                dijit.byId("ckObs").set("checked", true);
                dijit.byId("ckFollow").set("checked", true);
                dijit.byId("ckItem").set("checked", true);

                document.getElementById('trAvalEstagio').style.display = 'block';


            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}

function toglleTpAvalEstagio(value) {
    if (value == true)
        document.getElementById('trAvalEstagio').style.display = 'block';
    else
        document.getElementById('trAvalEstagio').style.display = 'none';

}

function setoptions(item, option) {
    option.selected = true;
    require(["dojo/on"], function (on) {
        if (option.value >= 0) {
            on(item, "click", function (e) {
                var id = item.parent.id.split("_");
                var checkedMultiSelect = dijit.byId(id[0]);

                var contSelected = 0;
                for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                    if (checkedMultiSelect.options[i].selected == true) {
                        contSelected++;
                    }
                }

                if (option.value == 0) {
                    for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                        checkedMultiSelect.options[i].selected = option.selected;
                    }
                } else {
                    if (checkedMultiSelect.options.length - 1 == contSelected &&
                        checkedMultiSelect.options[0].selected == false) {
                        for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                            checkedMultiSelect.options[i].selected = true;
                        }
                    } else {
                        checkedMultiSelect.options[0].selected = false;
                    }
                }
            });
        }
    });
}

function pesquisaAlunoFKRel() {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        
        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoRelFK() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dijit.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELMATRICULA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKRel();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montarparametrosmulti(componente) {
    var produtos = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtos = "100";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtos = "0"
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtos == "") {
                    produtos = dijit.byId(componente).value[i];
                } else {
                    produtos = produtos + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtos;
}



function emitirRelatorio() {
    try {


        if (hasValue(dojo.byId("noAluno").value)) {

            this.historico = montarparametrosmulti('cbHistorico');
            this.avaliacao = montarparametrosmulti('cbAvaliacao');
            this.avalEstagio = dojo.byId("ckEstagio").checked ? montarparametrosmulti('cbAvalEstagio') : "100";
            this.titulos = montarparametrosmulti('cbTitulos');
            this.mostrarEstagio =  dojo.byId("ckEstagio").checked ? true : false;
            this.mostrarAtividade = dojo.byId("ckAtividade").checked ? true : false;
            this.mostrarObservacao = dojo.byId("ckObs").checked ? true : false;
            this.mostrarFollow = dojo.byId("ckFollow").checked ? true : false;
            this.mostrarItem = dojo.byId("ckItem").checked ? true : false;
            this.cd_aluno = hasValue(dijit.byId("noAluno").cd_pessoa)
                ? dijit.byId("noAluno").cd_pessoa
                : (hasValue(dijit.byId("noAluno").value) ? dijit.byId("noAluno").value : null);
            this.no_aluno = dojo.byId('noAluno').value;
            
            var HistoricoAluno = function(historico, avaliacao, avalEstagio, titulos, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno) {
                this.historico = historico;
                this.avaliacao = avaliacao;
                this.avalEstagio = avalEstagio;
                this.titulos = titulos;
                this.mostrarEstagio = mostrarEstagio;
                this.mostrarAtividade = mostrarAtividade;
                this.mostrarObservacao = mostrarObservacao;
                this.mostrarFollow = mostrarFollow;
                this.mostrarItem = mostrarItem;
                this.cd_aluno = cd_aluno;
                this.no_aluno = no_aluno;
            };

            /*Historico Aluno*/
            historicoAluno = new HistoricoAluno(historico, avaliacao, avalEstagio, titulos, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno);

           
            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/GetUrlRelatorioHistoricoAluno",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    produtos : historicoAluno.historico,
                    turmaAvaliacao : historicoAluno.avaliacao,
                    estagioAvaliacao : historicoAluno.avalEstagio,
                    statusTitulo : historicoAluno.titulos,
                    mostrarEstagio : historicoAluno.mostrarEstagio,
                    mostrarAtividade : historicoAluno.mostrarAtividade,
                    mostrarObservacao : historicoAluno.mostrarObservacao,
                    mostrarFollow : historicoAluno.mostrarFollow,
                    mostrarItem : historicoAluno.mostrarItem,
                    cd_aluno: historicoAluno.cd_aluno,
                    no_aluno: historicoAluno.no_aluno
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function(data) {
                    abrePopUp(Endereco() + '/Relatorio/RelatorioHistoricoAluno?' + data,
                        '1024px',
                        '750px',
                        'popRelatorio');
                },
                function(error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        } else {
            showMessage('apresentadorMensagem',MENSAGEM_ERRO, "O Campo Aluno é obrigatório." );
        }

    }
    catch (e) {
        postGerarLog(e);
    }
}

function showMessage(el, tipoErro, msg) {
    var mensagensWeb = new Array();
    apresentaMensagem(el, null);
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msg);
    apresentaMensagem(el, mensagensWeb);
}




function loadProduto(idPeriodo, ItemFileReadStore) {
    
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getProdutosComHistorico",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            var produtos = JSON.parse(data).retorno;
            setProduto(produtos, 'cbHistorico');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {

    } 
};

function setProduto(produtos, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function(Memory, Array) {
            try {
                var dados = [];
                $.each(produtos, function (index, val) {
                    dados.push({
                        "name": val.no_produto,
                        "id": val.cd_produto
                    });
                    
                });

                var w = dijit.byId(field);
                //Adiciona a opção todos no checkedmultiselect
                dados.unshift({
                    "name": "Todos",
                    "id": 0
                });
                var storeProduto = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "id",
                        label: "name",
                        items: dados
                    }
                });
                if (produtos.length > 0) {


                    w.setStore(storeProduto, []);
                    setMenssageMultiSelect(PRODUTO, field);
                    dijit.byId(field).on("change",
                        function(e) {
                            setMenssageMultiSelect(PRODUTO, field, true, 20);
                        });
                };
            } catch (e) {

            }
        });
};

function loadAvaliacao(idPeriodo, ItemFileReadStore) {
    var dados = [
        { name: "Todas", id: 0 },
        { name: "Média/Notas", id: 1 },
        { name: "Conceito", id: 2 },
        { name: "Eventos/Última Aula", id: 3 }
    ]
    var stateStore = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    dijit.byId(idPeriodo).setStore(stateStore, []);
    setMenssageMultiSelect(TPAVALIACAO, idPeriodo);
    dijit.byId(idPeriodo).on("change", function (e) {
        setMenssageMultiSelect(TPAVALIACAO, idPeriodo, true, 20);
    });
}

function loadTitulos(idPeriodo, ItemFileReadStore) {
    var dados = [
        { name: "Todos", id: 0 },
        { name: "Abertos", id: 1 },
        { name: "Fechados", id: 2 },
        { name: "Cancelados", id: 3 }
    ]
    var stateStore = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    dijit.byId(idPeriodo).setStore(stateStore, []);
    setMenssageMultiSelect(TITULOS, idPeriodo);
    dijit.byId(idPeriodo).on("change", function (e) {
        setMenssageMultiSelect(TITULOS, idPeriodo, true, 20);
    });
}

function loadAvalEstagio(idPeriodo, ItemFileReadStore) {
    var dados = [
        { name: "Todas", id: 0 },
        { name: "Médias/Notas", id: 1 },
        { name: "Conceito", id: 2 }
    ]
    var stateStore = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    dijit.byId(idPeriodo).setStore(stateStore, []);
    setMenssageMultiSelect(TPAVALIACAO, idPeriodo);
    dijit.byId(idPeriodo).on("change", function (e) {
        setMenssageMultiSelect(TPAVALIACAO, idPeriodo, true, 20);
    });
}



