var HAS_PRODUTO = 4;

var EnumTipoAtividade = {
    AULAEXPERIMENTAL: 600
};

var EnumTipoRecorrencia = {
    DIARIAMENTE: 1,
    SEMANALMENTE: 2,
    QUINZENALMENTE: 3,
    MENSALMENTE: 4,
    ANUALMENTE: 5
}
//#region métodos auxiliares para o funcionamento do formulário  - mostraTabs - maskDate - maskHour - cleanUsarCpf - clearFormAtividade - limparFormAtividadeExtra -  keepValues - loadCursoByProduto(cdProduto)

//Monta máscaras para telefone, cpf e cnpj
function mascarar() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try{
                //maskDate("#dataAtividade");
                //maskDate("#dtaIni");
                //maskDate("#dtaFim");
                $("#hrInicial").mask("99:99");
                $("#hrFinal").mask("99:99");
                $("#hrLimite").mask("99:99");
                $("#hhIni").mask("99:99");
                $("#hhFim").mask("99:99");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function cleanUsarCpf() {
    try{
        dojo.byId("cdPessoaCpf").value = "";
        dojo.byId("nomPessoaCpf").value = "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearFormAtividade() {
    try{
        dijit.byId("cbSalas").reset();
        dijit.byId("cbResponsaveis").reset();
        dijit.byId("cbProdutos").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function changeActive(ativa) {
    dijit.byId('hrInicial')._onChangeActive = ativa;
    dijit.byId('hrFinal')._onChangeActive = ativa;
    dijit.byId('hrLimite')._onChangeActive = ativa;
    dijit.byId('dataAtividade')._onChangeActive = ativa;
    dijit.byId("cbProdutos")._onChangeActive = ativa;
    dijit.byId("cbSalas")._onChangeActive = ativa;
}
function limparFormAtividadeExtra(ready, Memory, filteringSelect, ObjectStore) {
    try{
    changeActive(false);
        dijit.byId("cbSalas").reset();
        dojo.byId('cd_atividade_extra').value = 0;
        dojo.byId('cd_pessoa_escola').value = 0;
        dijit.byId('cbTiposAtividades').reset();
        dijit.byId("cbResponsaveis").reset();
        dijit.byId("cbProdutos").reset();
        if (hasValue(dojo.byId('cbCursos').value, true))
            dijit.byId("cbCursos").reset();
        if (hasValue(dijit.byId('cbCursos')) && hasValue(dijit.byId('cbCursos').store))
            dijit.byId('cbCursos').setStore(dijit.byId('cbCursos').store, [0]);
        dijit.byId('cbCursos').reset();
        dijit.byId('ckCarga').set('value', false);
        dijit.byId('ckPagar').set('value', false);
        dijit.byId('ckPortal').set('value', false);
        dijit.byId('hrLimite').reset();
        dijit.byId("nroVagas").reset();
        dijit.byId("hrInicial").reset();
        dijit.byId("hrFinal").reset();
        dojo.byId("txObs").value = "";
        dijit.byId('dataAtividade').set('value', new Date());
        apresentaMensagem("apresentadorMensagemAtividadeExtra", "");

        var gridAlunoAtividade = dijit.byId("gridAtividadesAlunos");
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });

        gridAlunoAtividade.setStore(dataStore);
        gridAlunoAtividade.update();
    changeActive(true);

    }
    catch (e) {
        postGerarLog(e);
    }
}

//Pega os Antigos dados do Formulário
function keepValues(val, grid, ehLink) {
    try {
        var value = grid.itemSelecionado;
        var linkAnterior = document.getElementById('link');
        var gridAlunoAtv = dijit.byId('gridAtividadesAlunos');
        dojo.byId('abriuEscola').value = false;

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;
        if (value != null && value.cd_atividade_extra > 0) {

            changeActive(false);

            dojo.byId("cd_atividade_extra").value = value.cd_atividade_extra;
            dojo.byId("cd_pessoa_escola").value = value.cd_pessoa_escola;
            dojo.byId("nroVagas").value = value.nm_vagas;
            dijit.byId("hrInicial").value = value.hh_inicial != null ? dojo.byId("hrInicial").value = value.hh_inicial.replace(":00", '') : "";
            dijit.byId("hrFinal").value = value.hh_final != null ? dojo.byId("hrFinal").value = value.hh_final.replace(":00", '') : "";
            dijit.byId("dataAtividade").value = value.dta_atividade_extra != null ? dojo.byId("dataAtividade").value = value.dta_atividade_extra : "";
            dojo.byId('cdUsuario').value = value.cd_usuario_atendente;

            if (hasValue(dijit.byId('cbTiposAtividades'), true))
                hasValue(value.cd_tipo_atividade_extra) ? dijit.byId('cbTiposAtividades').set("value", value.cd_tipo_atividade_extra) : 0;

            verificaAulaExperimental();

            if (hasValue(dijit.byId('cbProdutos'), true))
                if (hasValue(dijit.byId('cbProdutos'), true)) {
                    dijit.byId('cbProdutos')._onChangeActive = false;
                    hasValue(value.cd_produto) ? dijit.byId('cbProdutos').set("value", value.cd_produto) : dijit.byId('cbProdutos').reset();
                    dijit.byId('cbProdutos')._onChangeActive = true;
                }

            //if (hasValue(dijit.byId('cbCursos'), true))
            //    hasValue(value.cd_curso) ? dijit.byId('cbCursos').set("value", value.cd_curso) : dijit.byId('cbCursos').reset();

            if (hasValue(dijit.byId('cbResponsaveis'), true))
                hasValue(value.cd_funcionario) ? dijit.byId('cbResponsaveis').set("value", value.cd_funcionario) : 0;

            limparGrid();

            if (hasValue(dijit.byId('cbSalas'), true)) {
                if (value.cd_sala != null && value.cd_sala != undefined && value.cd_sala > 0) {
	                dijit.byId('cbSalas').set("value", value.cd_sala);
                }

                if (hasValue(dijit.byId("cbSalas").value)) {
                    verificaSalaOnline(value.cd_pessoa_escola == dojo.byId('_ES0').value);
                    popularGridEscolaAtividade();
                }

                var parametros = getParamterosURL();
                if (!eval(parametros['lancamento']))
                    dijit.byId("cbSalas").set("disabled", false);
            } else {
                dojo.byId("tagEscola").style.display = "none";
            }

            dijit.byId('ckCarga').set('value', value.ind_carga_horaria);
            dijit.byId('ckPagar').set('value', value.ind_pagar_professor);

            dijit.byId('ckPortal').set('value', value.id_calendario_academico);
            dijit.byId('hrLimite').value = value.hr_limite_academico != null ? dojo.byId("hrLimite").value = value.hr_limite_academico.replace(":00", '') : "";

            if (hasValue(value.tx_obs_atividade))
                dojo.byId("txObs").value = value.tx_obs_atividade;
            else dijit.byId("txObs").set('value', '');

            // método para carregar a pessoa logada no sistema
            // Podemos ter vários usuarios persitindo o formulário, no caso se o registro for editado, o usuario que alterar ou deletar o registro pode ser diferente do usuario que inseriu o registro, por isso dois campos de ID.
            dojo.byId('txAtendente').value = value.no_usuario;
            changeActive(true);
            pesquisarAlunosAtividades();
            
        }
        dojo.byId('abriuEscola').value = false;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadCursoByProduto(cdProduto) {
    require([
        "dojo/_base/xhr"
    ], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/curso/getPesCursos?hasDependente=" + HAS_PRODUTO + "&cd_curso=null" + "&cd_produto=" + cdProduto,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() },
            idProperty: "cd_curso"
        }).then(function (dataReturn) {
            try {
                apresentaMensagem('apresentadorMensagemAtividadeExtra', null);

                if (dataReturn != null && dataReturn.retorno.length > 0) {
                    dijit.byId("cbCursos").reset();
                    loadMultiCurso(dataReturn.retorno, "cbCursos");
                    var parametros = getParamterosURL();
                    if (!eval(parametros['lancamento']))
                        dijit.byId("cbCursos").set('disabled', false);
                } else {
                    dijit.byId("cbCursos").set('disabled', true);
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nenhum curso foi encontrado.");
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
        });
    });
}
//#endregion

//#region formatação do checks das grids
function formatCheckBoxAtividadeExtra(value, rowIndex, obj) {
    try
    {
        var gridName = 'gridAtividadeExtra';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAtividadeExtra');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_atividade_extra", grid._by_idx[rowIndex].item.cd_atividade_extra);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_atividade_extra', 'selecionadoAtividadeExtra', -1, 'selecionaTodosAtividadeExtra', 'selecionaTodosAtividadeExtra', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_atividade_extra', 'selecionadoAtividadeExtra', " + rowIndex + ", '" + id + "', 'selecionaTodosAtividadeExtra', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}



//Escola
function formatCheckBoxEscolaAtividadeExtra(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscola');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEscola', -1, 'selecionaTodosEscola', 'selecionaTodosEscola', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEscola', " + rowIndex + ", '" + id + "', 'selecionaTodosEscola', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatCheckBoxAtividadesAlunos(value, rowIndex, obj) {
    try{
        var gridName = 'gridAtividadesAlunos';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAtividadesAlunos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);
            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAtividadesAlunos', -1, 'selecionaTodosAtividadesAlunos', 'selecionaTodosAtividadesAlunos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionadoAtividadesAlunos', " + rowIndex + ", '" + id + "', 'selecionaTodosAtividadesAlunos', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxParticipacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAtividadesAlunos';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input disabled='disabled' class='formatCheckBox' id='" + id + "'/> ";

        setTimeout("configuraCheckBoxParticipou(" + value + ", '" + id + "', '" + gridName + "', 'disabled')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatProspect(value, rowIndex, obj) {
    try {
        var gridName = 'gridAtividadesAlunos';
        var grid = dijit.byId(gridName);
        var icon;
        if (rowIndex != -1 && value)
            icon = "<b style='font-family:'Tahoma''>P</b>";
        else
            icon = "<label></label>";
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxParticipou(value, id, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try{
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);


                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (disabled == null || disabled == undefined) disabled = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: disabled
                    }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
//#endregion

//#region montar os dropsdown
function getReturnDataAtividadeExtra(fieldTipoAtividade, fieldCurso, fieldProfessor, fieldProdudo, fieldAluno, fieldSala) {
    var atividadeExtraPesq = new AtividadeExtraPesquisa(null);
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/returnDataAtividadeExtra", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(atividadeExtraPesq)
        }).then(function (dataAtividadeExtra) {
            try{
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, fieldTipoAtividade);
                    loadProfessor(dataAtividadeExtra.retorno.professores, fieldProfessor);
                    loadProduto(dataAtividadeExtra.retorno.produtos, fieldProdudo);
                   // fieldAluno != null ? loadAluno(dataAtividadeExtra.retorno.alunos, fieldAluno) : "";
                    fieldSala != null ? loadSala(dataAtividadeExtra.retorno.salas, fieldSala) : "";
                    // método para carregar a pessoa logada no sistema
                    // Podemos ter vários usuarios persitindo o formulário, no caso se o registro for editado, o usuario que alterar ou deletar o registro pode ser diferente do usuario que inseriu o registro, por isso dois campos de ID.
                    dojo.byId('txAtendente').value = dataAtividadeExtra.retorno.no_usuario;
                    dojo.byId('cdUsuario').value = dataAtividadeExtra.retorno.cd_usuario_atendente;
                } else
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    });
}

//Método para pesquisa
function returnDataAtividadeExtraParaPesquisa(fieldTipoAtividade, fieldCurso, fieldProfessor, fieldProdudo, fieldAluno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/returnDataAtividadeExtraParaPesquisa", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(null)
        }).then(function (dataAtividadeExtra) {
            try {
                dataAtividadeExtra = jQuery.parseJSON(dataAtividadeExtra);
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, fieldTipoAtividade);
                    loadCurso(dataAtividadeExtra.retorno.cursos, fieldCurso);
                    loadProfessor(dataAtividadeExtra.retorno.professores, fieldProfessor);
                    loadProduto(dataAtividadeExtra.retorno.produtos, fieldProdudo);
                    //loadAluno(dataAtividadeExtra.retorno.alunos, fieldAluno);
                } else
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgLockPermissionSearchFilter);

            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            //apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    });

}

function loadTipoAtividadeExtra(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
  function (Memory, Array) {
      try{
          var itemsCb = [];
          var cbTipoAtividade = dijit.byId(field);
          Array.forEach(items, function (value, i) {
              itemsCb.push({ id: value.cd_tipo_atividade_extra, name: value.no_tipo_atividade_extra });
          });
          var stateStore = new Memory({
              data: itemsCb
          });
          cbTipoAtividade.store = stateStore;
      }
      catch (e) {
          postGerarLog(e);
      }
  });
}
//fim  Métodos Tipo Atividade


function loadCurso(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbCurso = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_curso, name: value.no_curso });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbCurso.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function loadMultiCurso(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var dados = [];
            var cbCurso = [];                   
            $.each(items, function (index, val) {
                dados.push({
                    "name": val.no_curso,
                    "id": val.cd_curso
                });
                cbCurso.push({
                    "cd_curso": val.cd_curso + "",
                    "no_curso": val.no_curso
                });
            });

            var w = dijit.byId(field);
            //Adiciona a opção todos no checkedmultiselect
            cbCurso.unshift({
                "cd_curso": -1 + "",
                "no_curso": "Todos"
            });
            var storeMCurso = new dojo.data.ItemFileReadStore({
                data: {
                    identifier: "cd_curso",
                    label: "no_curso",
                    items: cbCurso
                }
            });
            w.setStore(storeMCurso, []);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//retorno do multicheckbox
function checkedValuesTrue(cursos, cd_cursos, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            dijit.byId('cbCursos').reset();
            getValues();

            for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
                for (var j = 0; j < cd_cursos.length; j++) {
                    if (dijit.byId("cbCursos").options[i].item.cd_curso[0] == cd_cursos[j]) {
                        console.log(dijit.byId("cbCursos").options[i].item.cd_curso[0] == cd_cursos[j]);
                        dijit.byId("cbCursos").options[i].selected = true;
                        dijit.byId("cbCursos").onChange(true);
                    }
                }
            }
        }
        catch (e) {
            //showCarregando();
            postGerarLog(e);
        }
    });
}

// fIm dos métodos do Curso

function loadProfessor(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbResponsavel = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_pessoa, name: value.no_pessoa });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbResponsavel.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//fim do procedimeto para Professor

function loadProduto(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbProduto = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_produto, name: value.no_produto });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbProduto.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
// fim do procedimento para produto

// método para sala
function populaSala(field, horaIni, horaFim, data) {
    try{
        apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
        data = hasValue(dojo.byId("dataAtividade").value) ? dojo.byId("dataAtividade").value : 0;//dateFormat(data, 'dd/mm/yyyy');
        horaIni = hasValue(dojo.byId("hrInicial").value) ? dojo.byId("hrInicial").value : null;//dateFormat(horaIni, 'HH:MM');
        horaFim = hasValue(dojo.byId("hrFinal").value) ? dojo.byId("hrFinal").value : null;//dateFormat(horaFim, 'HH:MM');
        var idAtividadeExtra = hasValue(dojo.byId('cd_atividade_extra').value > 0) ? dojo.byId('cd_atividade_extra').value : null;

 		if (!dijit.byId("hrFinal").validate() || !dijit.byId("hrInicial").validate() || !dijit.byId("dataAtividade").validate())
       		return false;
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getSalaHorariosDisponiveis?horaIni=" + horaIni + "&horaFim=" + horaFim + "&data=" + data + "&cd_atividade_extra=" + idAtividadeExtra,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataSala) {
                try{
                    var dados = $.parseJSON(dataSala);
                    if (!hasValue(dados.erro) && dados.retorno.length > 0 && dados.MensagensWeb.length == 0) {
                        apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                        loadSala(dados.retorno, field);
                    } else {
                        var mensagensWeb = new Array();
                        apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, dados.MensagensWeb[0].mensagem);
                        apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                        dijit.byId("cbSalas").reset();
                        loadSala(dados.retorno, field);
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
                clearFormAtividade();
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSala(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbSala = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_sala, name: value.no_sala });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbSala.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
function loadLancada(nomElement, val) {
    try {
        var statusStore = [];
        //if(val != 0)
            statusStore = new dojo.store.Memory({
                data: [
                       { name: "Todas", id: 2 },
                       { name: "Não", id: 0 },
                       { name: "Sim", id: 1 }
                ]
            });
        //else
        //    statusStore = new dojo.store.Memory({
        //        data: [
        //               { name: "Não", id: 0 },
        //               { name: "Sim", id: 1 }
        //        ]
        //    });
        var status = new dijit.form.FilteringSelect({
            id: nomElement + '_1',
            name: nomElement + '_1',
            value: val,
            store: statusStore,
            searchAttr: "name",
            style: "width: 75px;"
        }, nomElement);
    }
    catch (e) {
        postGerarLog(e);
    }
};

// fim do procedimento para sala
//#endregion

//#region  Criação da Grades e métodos auxiliares - manipulaDropDownSala - montarObjetoAtividadeExtra
function montarCadastroAtividadeExtra() {
    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojo/data/ItemFileReadStore",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/FilteringSelect",
        "dojo/date",
        "dojo/on",
        "dojo/require",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, EnhancedGrid, ItemFileReadStore, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, filteringSelect, date) {
        ready(function () {
            try {
                //findAllEmpresasUsuarioComboFiltroEscolaTelaAtividadeExtra();
                mascarar();
                var registroGrid = null;
                var desabilitaCamposLancamento = false;
                dijit.byId("cbCursos").dropDownMenu.domNode.style.maxHeight = "300px";

                setMenssageMultiSelect(CURSO, 'cbCursos');
                dijit.byId("cbCursos").on("change", function (e) {
                    setMenssageMultiSelect(CURSO, 'cbCursos');
                });

                pesquisarEscolasVinculadasUsuarioAtividade();
                if (hasValue(dijit.byId("cbSalas")) &&
                    dijit.byId("cbSalas").value > 0 &&
                    dojo.byId("qtdEscolasVinculadasUsuario").value > 0) {
                    verificaSalaOnline(true);
                } else {
                    dojo.byId("tagEscola").style.display = "none";
                }

                returnDataAtividadeExtraParaPesquisa('cbTipoAtividade', 'cbCurso', 'cbResponsavel', 'cbProduto', 'cbAluno');

                var parametros = getParamterosURL();
                var cdProf = null;
                loadLancada('cbLancada', eval(parametros['lancamento']) ? 0 : 2);
                var lancada = eval(parametros['lancamento']) ? 0 : 2;
                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (eval(parametros['lancamento'])) {
                            if (data.retorno != null && data.retorno.Professor != null && data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                                dojo.byId("cbResponsavel").value = data.retorno.Professor.no_fantasia;
                                dijit.byId("cbResponsavel").set('value', data.retorno.Professor.cd_pessoa);
                                dijit.byId("cbResponsavel").set('cd_pessoa', data.retorno.Professor.cd_pessoa);
                                dijit.byId("cbResponsavel").set("disabled", true);
                                cdProf = data.retorno.Professor.cd_pessoa;
                                if (eval(Master()))
                                    cdProf = null;
                                //LBM else Sempre vai desabilitar controles no lançamento
                                //    desabilitaCamposLancamento = true;
                            }
                            else if (!eval(Master()) && (data.retorno == null || (data.retorno.Professor != null && !data.retorno.Professor.id_coordenador))) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'O usuário que está lançando esta atividade não está vinculado a um Responsável.');
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);

                                //LBMdesabilitaCamposLancamento = true;
                                cdProf = 0;
                            }
                            dojo.byId('labelTitulos').innerHTML = 'Atividade Extra(Lançamento)'
                            dijit.byId('cadAtividadeExtra').set('title', 'Lançamento Atividade Extra');
                            desabilitaCamposLancamento = true; //LBM empre vai desabilitar controles no lançamento
                        } else {
                            dojo.byId('labelTitulos').innerHTML = 'Atividade Extra(Cadastro)';
                        }

                        var gridEscolas = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodosEscola' style='display:none'/>", field: "selecionadoEscola", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxEscolaAtividadeExtra },
                                { name: "Escola", field: "dc_reduzido_pessoa", width: "98%" }
                            ],
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "34", "68", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    maxPageStep: 4,
                                    position: "button",
                                    plugins: { nestedSorting: true }
                                }
                            }
                        }, "gridEscolas");
                        gridEscolas.canSort = function (col) { return Math.abs(col) != 1; };
                        gridEscolas.startup();

                        new Button({
                            label: "Incluir",
                            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                        montargridPesquisaPessoa(function () {
                                            dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                                if (e.keyCode == 13) pesquisarEscolasFK();
                                            });
                                            dijit.byId("pesqPessoa").on("click", function (e) {
                                                apresentaMensagem("apresentadorMensagemProPessoa", null);
                                                pesquisarEscolasFK();
                                            });
                                            abrirPessoaFK(false);
                                        });
                                    else
                                        abrirPessoaFK(false);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "incluirEscolaFK");

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_escola', gridEscolas);
                            }
                        });
                        menu.addChild(menuTodosItens);

                        if (!hasValue(dijit.byId('todosItensEscola'))) {
                            var button = new DropDownButton({
                                label: "Ações Relacionadas",
                                name: "todosItensEscola",
                                dropDown: menu,
                                id: "todosItensEscola"
                            });
                            dom.byId("linkSelecionadosEscola").appendChild(button.domNode);
                        }


                        var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/coordenacao/getAtividadeExtraSearch?dataIni=" + minDate + "&dataFim=" + maxDate + "&hrInicial=null&hrFinal=null&tipoAtividade=null&curso=null&responsavel=" + cdProf + "&produto=null&aluno=null&lancada=" + lancada + "&cd_escola_combo=0" ,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                idProperty: "cd_atividade_extra"
                            }
                        ), Memory({ idProperty: "cd_atividade_extra" }));

                        var gridAtividadeExtra = new EnhancedGrid({
                            store: ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodosAtividadeExtra' style='display:none'/>", field: "selecionadoAtividadeExtra", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividadeExtra },
                                { name: "Data", field: "dta_atividade_extra", width: "10%", styles: "text-align: left; min-width:75px; max-width:75px;" },
                                { name: "Hr. Inicial", field: "hh_inicial", width: "10%", styles: "text-align: center;" },
                                { name: "Hr. Final", field: "hh_final", width: "10%", styles: "text-align: center;" },
                                { name: "Tipo Atividade", field: "no_tipo_atividade_extra", width: "10%", styles: "text-align: left;" },
                                { name: "Produto", field: "no_produto", width: "10%", styles: "text-align: left;" },
                                { name: "Curso", field: "no_curso", width: "10%", styles: "text-align: left;" },
                                { name: "Responsável", field: "no_responsavel", width: "20%", styles: "text-align: left;" },
                                { name: "Vagas", field: "nm_vagas", width: "10%", styles: "text-align: center;" },
                                { name: "Alunos", field: "nm_alunos", width: "10%", styles: "text-align: center;" }
                            ],
                            noDataMessage: msgNotRegEncFiltro,
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "34", "68", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    maxPageStep: 4,
                                    position: "button",
                                    plugins: { nestedSorting: true }
                                }
                            }
                        }, "gridAtividadeExtra"); // make sure you have a target HTML element with this id
                        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                        gridAtividadeExtra.pagination.plugin._paginator.plugin.connect(gridAtividadeExtra.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridAtividadeExtra, 'cd_atividade_extra', 'selecionaTodosAtividadeExtra');
                        });
                        gridAtividadeExtra.startup();
                        gridAtividadeExtra.canSort = function (col) { return Math.abs(col) != 1; };
                        gridAtividadeExtra.on("RowDblClick", function (evt) {
                            try{
                                var idx = evt.rowIndex,
                                      item = this.getItem(idx),
                                      store = this.store;
                                registroGrid = item
                                //showCarregando();
                                apresentaMensagem('apresentadorMensagem', '');
                                gridAtividadeExtra.itemSelecionado = montarObjetoAtividadeExtra(item);
                                var existIdSala = gridAtividadeExtra.itemSelecionado.cd_sala ? true : false;
                                atividadeExtraPesq = new AtividadeExtraPesquisa(item);
                                dijit.byId("cbSalas").store.data = null;
                                
                                require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                                    xhr.post(Endereco() + "/coordenacao/getAtividadeExtraViewOnDbClik", {
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                        handleAs: "json",
                                        data: ref.toJson(atividadeExtraPesq)
                                    }).then(function (dataAtividadeExtra) {
                                        try {
                                            showCarregando();
                                            if (!hasValue(dataAtividadeExtra.erro)) {
                                                limparFormAtividadeExtra(ready, Memory, filteringSelect, ObjectStore);
                                                loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, "cbTiposAtividades");
                                                verificaAulaExperimental();
                                                //loadProfessor(dataAtividadeExtra.retorno.professores, "cbResponsaveis");
                                                //loadProduto(dataAtividadeExtra.retorno.produtos, "cbProdutos");
                                                loadMultiCurso(dataAtividadeExtra.retorno.cursos, "cbCursos");
                                                dojo.byId('nm_alunos').value = dataAtividadeExtra.retorno.nm_total_alunos;


                                                //if ((dataAtividadeExtra.retorno.cursos != null) && (dataAtividadeExtra.retorno.cursos.length > 0))
                                                //    dijit.byId("cbCursos").set('disabled', false);

                                                loadSala(dataAtividadeExtra.retorno.salasDisponiveis, "cbSalas");

                                                obterRecursosAtividadeExtraDbClick(atividadeExtraPesq, dataAtividadeExtra.retorno.cursos, gridAtividadeExtra, registroGrid.cd_pessoa_escola);
                                                
                                                
                                                //pesquisarEscolasVinculadasUsuarioAtividade();
                                                
                                                dojo.byId('abriuEscola').value = false;

                                            } else
                                                apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
                                            
                                        }
                                        catch (er) {
                                            postGerarLog(er);
                                        }                                        
                                    },
                                    function (error) {
                                        showCarregando();
                                        apresentaMensagem('apresentadorMensagem', error);
                                    });
                                });
                                dijit.byId("txAtendente").set("disabled", true);
                                IncluirAlterar(0, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagemAtividadeExtra', 'divCancelarAtividadeExtra', 'divLimparAtividadeExtra');
                            }
                            catch (e) {
                                postGerarLog(e);
                            }                            
                        }, true);

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridAtividadeExtra, "_onFetchComplete", function () {
                                // Configura o check de todos:
                                if (dojo.byId('selecionaTodosAtividadeExtra').type == 'text')
                                    setTimeout("configuraCheckBox(false, 'cd_atividade_extra', 'selecionadoAtividadeExtra', -1, 'selecionaTodosAtividadeExtra', 'selecionaTodosAtividadeExtra', 'gridAtividadeExtra')", gridAtividadeExtra.rowsPerPage * 3);
                            });
                        });

                        //***************** Adiciona link de ações:****************************\\
                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () {
                                eventoEditarAtividadeExtra(dijit.byId('gridAtividadeExtra').itensSelecionados, ready, Memory, filteringSelect, ObjectStore);
                            }
                        });
                        menu.addChild(acaoEditar);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                
                                if (gridAtividadeExtra.itensSelecionados.every(function (item) {
                                    return (item.cd_pessoa_escola + "") === dojo.byId("_ES0").value;
                                })) {
                                    eventoRemover(gridAtividadeExtra.itensSelecionados, 'DeletarAtividadeExtra(itensSelecionados)');
                                } else {
                                    caixaDialogo(DIALOGO_ERRO,
                                        "A funcionalidade para deletar atitividades não é permitida para atividades de escolas diferentes da escola logada. O(s) item(ns) selecionado(s) possue(em) atividade(s) de outra escola.",
                                        null);
                                    
                                }
                                
                            }
                        });
                        menu.addChild(acaoRemover);
                        //Acao gerarRecorrencia
                        var acaoRecorrencia = new MenuItem({
                            label: "Gerar Recorrência",
                            onClick: function () {
                                eventoGerarRecorrenciaAtividadeExtra(dijit.byId('gridAtividadeExtra').itensSelecionados, ready, Memory, filteringSelect, ObjectStore);
                                
                            }
                        });
                        var urlParametro = getParamterosURL();
                        if (!eval(urlParametro['lancamento'])) {
                            menu.addChild(acaoRecorrencia);
                        }

                        var acaoEnviarEmailRecorrencia = new MenuItem({
                            label: "Enviar Email",
                            onClick: function () {
                                EnviarEmailProspectsAcaoRelacionada(dijit.byId('gridAtividadeExtra').itensSelecionados);

                            }
                        });
                        var urlParametro = getParamterosURL();
                        if (!eval(urlParametro['lancamento'])) {
                            menu.addChild(acaoEnviarEmailRecorrencia);
                        }
                       
                        //fim acao gerarRecorrencia

                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasAtividadeExtra",
                            dropDown: menu,
                            id: "acoesRelacionadasAtividadeExtra"
                        });
                        dom.byId("linkAcoesAtividadeExtra").appendChild(button.domNode);

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () { buscarTodosItens(gridAtividadeExtra, 'todosItensAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra']); pesquisarAtividadeExtra(false,lancada); }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridAtividadeExtra', 'selecionadoAtividadeExtra', 'cd_atividade_extra', 'selecionaTodosAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra'); }
                        });
                        menu.addChild(menuItensSelecionados);

                        var button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItensAtividadeExtra",
                            dropDown: menu,
                            id: "todosItensAtividadeExtra"
                        });
                        dom.byId("linkSelecionadosAtividadeExtra").appendChild(button.domNode);

                        //*** Cria os botões de persistência **\\

                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function () {
                                IncluirAtividadeExtra();
                            }
                        }, "incluirAtividadeExtra");
                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function () {
                                AlteraAtividadeExtra();
                            }
                        }, "alterarAtividadeExtra");
                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                            onClick: function () {
                                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                                    DeletarAtividadeExtra();
                                });
                            }
                        }, "deleteAtividadeExtra");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                            onClick: function () {
                                limparFormAtividadeExtra(ready, Memory, filteringSelect, ObjectStore);
                            }
                        }, "limparAtividadeExtra");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                            onClick: function () {
                                try{
                                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                                    dijit.byId('hrInicial')._onChangeActive = false;
                                    dijit.byId('hrFinal')._onChangeActive = false; 
                                    dijit.byId('dataAtividade')._onChangeActive = false;
                                    dijit.byId('hrLimite')._onChangeActive = false;
                                    var existIdSala = gridAtividadeExtra.itemSelecionado.cd_sala ? true : false;
                                    atividadeExtraPesq = new AtividadeExtraPesquisa(registroGrid);
                                    dijit.byId("cbSalas").store.data = null;
                                    showCarregando();
                                    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
                                        xhr.post(Endereco() + "/coordenacao/getAtividadeExtraViewOnDbClik", {
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                            handleAs: "json",
                                            data: ref.toJson(atividadeExtraPesq)
                                        }).then(function (dataAtividadeExtra) {
                                            if (!hasValue(dataAtividadeExtra.erro)) {
                                                loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, "cbTiposAtividades");
                                                loadMultiCurso(dataAtividadeExtra.retorno.cursos, "cbCursos");
                                                //loadProfessor(dataAtividadeExtra.retorno.professores, "cbResponsaveis");
                                                //loadProduto(dataAtividadeExtra.retorno.produtos, "cbProdutos");
                                                var salas = dataAtividadeExtra.retorno.salasDisponiveis;
                                                loadSala(salas, "cbSalas");

                                                obterRecursosAtividadeExtraClickCancel(atividadeExtraPesq, dataAtividadeExtra.retorno.cursos, gridAtividadeExtra);
                                                
                                            } else
                                                apresentaMensagem('apresentadorMensagemAtividadeExtra', data);

                                            hideCarregando();
                                        },
                                        function (error) {
                                            showCarregando();
                                            apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
                                        });
                                    });
                                    dijit.byId('hrInicial')._onChangeActive = true;
                                    dijit.byId('hrFinal')._onChangeActive = true;
                                    dijit.byId('hrLimite')._onChangeActive = true;
                                    dijit.byId('dataAtividade')._onChangeActive = true;
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cancelarAtividadeExtra");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function () {
                                dijit.byId("cadAtividadeExtra").hide();
                            }
                        }, "fecharAtividadeExtra");
                        new Button({
                            label: "",
                            iconClass: 'dijitEditorIconSearchSGF',
                            onClick: function () {
                                apresentaMensagem('apresentadorMensagem', null);
                                pesquisarAtividadeExtra(true,lancada);
                            }
                        }, "pesquisarAtividadeExtra");
                        decreaseBtn(document.getElementById("pesquisarAtividadeExtra"), '32px');

                        //Botoes Recorrência
                        new Button({
                            label: "Gerar", iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                                GerarRecorrenciaAtividadeExtra(dijit.byId('gridAtividadeExtra').itensSelecionados[0]);
                            }
                        }, "gerarRecorrencia");

                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                                eventoRemover(gridAtividadeExtra.itensSelecionados, 'DeletarRecorrencias(itensSelecionados)');
                                
                            }
                        }, "DeleteRecorrencia");

                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                dijit.byId('dialogGerarRecorrencia').hide();
                            }
                        }, "fecharDialogRecorrencia");

                        new Button({
                            label: "Enviar E-mail", iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                                if (!hasValue(gridAtividadeExtra.itensSelecionados) || gridAtividadeExtra.itensSelecionados.length <= 0) {
                                    caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para enviar email.', null);
                                }else if (gridAtividadeExtra.itensSelecionados.length > 1) {
                                    caixaDialogo(DIALOGO_AVISO, 'Selecione apena um registro para enviar email.', null);
                                } else {
                                    caixaDialogo(DIALOGO_CONFIRMAR, 'Confirma o envio de email das recorrências geradas', function () {
                                        EnviarEmailRecorrencias(gridAtividadeExtra.itensSelecionados);
                                    });
                                }
                                
                            }
                        }, "enviarEmailAllRecorrencias");

                        //new Button({
                        //    label: "Enviar E-mail(Rec. Atual)", iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        //        //GerarRecorrenciaAtividadeExtra(dijit.byId('gridAtividadeExtra').itensSelecionados[0]);
                        //    }
                        //}, "enviarEmailRecorrencia");
                        //fim Botoes Recorrencia


                        //Botões do dialogo aluno/atividades
                        new Button({
                            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                            onClick: function () {
                                alterarAvaliacaoGrid(dijit.byId("gridAtividadesAlunos"));
                                dijit.byId("dialogAluno").hide();
                            }
                        }, "incluirAluno");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                            type: "reset",
                            onClick: function () {
                                document.getElementById("descObs").value = "";
                                dojo.byId("peso").checked = false;
                            }
                        }, "limparAluno");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                            onClick: function () {
                                keepValuesAluno(dijit.byId("gridAtividadesAlunos"));
                            }
                        }, "cancelarAluno");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function () {
                                dijit.byId("dialogAluno").hide();
                            }
                        }, "fecharAluno");

                        new Button({
                            label: "Incluir Aluno", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFk();
                                    });
                                }
                                else
                                    abrirAlunoFk();
                            }
                        }, "incluirAlunoFK");

                        new Button({
                            label: "Incluir Prospect", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("fecharProspectFK"))) {
                                        montarGridPesquisaProspect(true, function () {
                                            abrirFKProspectOrigemAluno();
                                            dojo.query("#nomeProspectFK").on("keyup", function (e) {
                                                if (e.keyCode == 13) pesquisarProspectFK();
                                            });
                                            dijit.byId("fecharProspectFK").on("click", function (e) {
                                                dijit.byId("cadProspectFollowUpFK").hide();
                                            });
                                        }, true);
                                    }
                                    else
                                        abrirFKProspectOrigemAluno();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "incluirProspectFK");

                        if (eval(parametros['lancamento'])) {
                            dijit.byId("incluirAlunoFK").set('disabled', true);
                            dijit.byId("incluirProspectFK").set('disabled', true);
                        }
                        else {
                            //dojo.byId('trEscolaFiltroTelaAtividadeExtra').style.display = "none";
                            dijit.byId("ckParticipouAluno").set('disabled', true);
                            dijit.byId("descObs").set('disabled', false);
                        }
                        //fim

                        new Button({
                            label: "Novo",
                            iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                            onClick: function () {
                                //Limpa a grade
                                showCarregando();
                                var gridAlunoAtividade = dijit.byId("gridAtividadesAlunos");
                                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                            dijit.byId("cbCursos").set('disabled', true);
                                xhr.get({
                                    url: Endereco() + "/coordenacao/returnDataAtividadeExtra",
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (dataTipoAtividadeExtra) {
                                    try {
                                        dijit.byId("tagAlunos").setTitle("Alunos/Clientes");
                                        require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                                            domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', 'none');
                                        });

                                        dijit.byId('hrInicial')._onChangeActive = true;
                                        dijit.byId('hrFinal')._onChangeActive = true;
                                        dijit.byId('hrLimite')._onChangeActive = true;
                                        dijit.byId('dataAtividade')._onChangeActive = true;
                                        limparFormAtividadeExtra(ready, Memory, filteringSelect, ObjectStore);
                                        dijit.byId("cbSalas").store.data = null;
                                        dijit.byId("tagAlunos").set("open", false);
                                        apresentaMensagem('apresentadorMensagem', null);
                                        dijit.byId('txAtendente').set('disabled', true);
                                        IncluirAlterar(1, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagemAtividadeExtra', 'divCancelarAtividadeExtra', 'divLimparAtividadeExtra');
                                        getReturnDataAtividadeExtra("cbTiposAtividades", "cbCursos", "cbResponsaveis", "cbProdutos", null, null);
                                        dojo.addOnLoad(function () {
                                            dijit.byId('dataAtividade').attr("value", new Date(ano, mes, dia));
                                        });
                                        if (hasValue(dijit.byId("gridAtividadesAlunos"))) {
                                            dijit.byId("gridAtividadesAlunos").update();
                                        }

                                        gridAlunoAtividade.setStore(dataStore);
                                        gridAlunoAtividade.update();
                                        if (hasValue(dataTipoAtividadeExtra) && hasValue(dataTipoAtividadeExtra.retorno)) {
                                            if (hasValue(dataTipoAtividadeExtra.retorno.tiposAtividadeExtras))
                                                loadTipoAtividadeExtra(dataTipoAtividadeExtra.retorno.tiposAtividadeExtras, "cbTiposAtividades");
                                            if (hasValue(dataTipoAtividadeExtra.retorno.cursos))
                                                loadMultiCurso(dataTipoAtividadeExtra.retorno.cursos, "cbCursos");
                                            if (hasValue(dataTipoAtividadeExtra.retorno.professores))
                                                loadProfessor(dataTipoAtividadeExtra.retorno.professores, "cbResponsaveis");
                                            if (hasValue(dataTipoAtividadeExtra.retorno.produtos))
                                                loadProduto(dataTipoAtividadeExtra.retorno.produtos, "cbProdutos");
                                            dojo.byId('txAtendente').value = dataTipoAtividadeExtra.retorno.no_usuario;
                                            dojo.byId('cdUsuario').value = dataTipoAtividadeExtra.retorno.cd_usuario_atendente;
                                        }

                                        limparGrid();
                                        if (hasValue(dijit.byId("cbSalas").value)) {
                                            verificaSalaOnline(true);
                                            popularGridEscolaAtividade();
                                        } else {
                                            dojo.byId("tagEscola").style.display = "none";
                                        }

                                        pesquisarEscolasVinculadasUsuarioAtividade();
                                        dojo.byId('abriuEscola').value = false;

                                        dijit.byId("cadAtividadeExtra").show();
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                    showCarregando();
                                },
                                function (error) {
                                    showCarregando();
                                    apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
                                });
                            }
                        }, "novaAtividadeExtra");

                        //----Monta o botão de Relatório----
                        var variaveisRelatorio = new VariaveisPesquisa(lancada);
                        new Button({
                            label: getNomeLabelRelatorio(),
                            iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                            onClick: function () {
                                try{
                                    if (!hasValue(gridAtividadeExtra.itensSelecionados) || gridAtividadeExtra.itensSelecionados.length < 1) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione alguma atividade extra para emitir o relatório.');
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                    }
                                    else
                                        if (gridAtividadeExtra.itensSelecionados.length > 1) {
                                            var mensagensWeb = new Array();
                                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione somente uma atividade extra para emitir o relatório.');
                                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                        }
                                        else {
                                            apresentaMensagem('apresentadorMensagem', null);
                                            var cdAtividadeExtra = gridAtividadeExtra.itensSelecionados[0].cd_atividade_extra;

                                            require(["dojo/_base/xhr"], function (xhr) {
                                                xhr.get({
                                                    url: Endereco() + "/api/coordenacao/GetUrlRelatorioAtividadeExtra?cdAtividadeExtra=" + cdAtividadeExtra + "&cd_escola_combo=0",
                                                    preventCache: true,
                                                    handleAs: "json",
                                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                                }).then(function (data) {
                                                    abrePopUp(Endereco() + '/Relatorio/RelatorioAtividadeExtra?' + data, '765px', '771px', 'popRelatorio');
                                                },
                                                function (error) {
                                                    apresentaMensagem('apresentadorMensagem', error);
                                                });
                                            })
                                        }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "relatorioAtividadeExtra");

                        // botões de pesquisa de aluno
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFk();
                                    });
                                }
                                else
                                    abrirAlunoFk();
                            }
                        }, "pesAluno");

                        new Button({
                            label: "limpar", iconClass: '', type: "reset",
                            onClick: function () {
                                dojo.byId('alunoPesq').value = '';
                                dojo.byId('cdAlunoPesq').value = null;
                            }
                        }, "limparAlunoPesc");

                        if (hasValue(document.getElementById("limparAlunoPesc"))) {
                            document.getElementById("limparAlunoPesc").parentNode.style.minWidth = '40px';
                            document.getElementById("limparAlunoPesc").parentNode.style.width = '40px';
                        }

                        //Altera o tamanho dos botões
                        var buttonFkArray = ['pesAluno'];
                        diminuirBotoes(buttonFkArray);
                        //Grid de aluno atividade
                        var data = new Array();
                        var gridAtividadesAlunos = new EnhancedGrid({
                            store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_atividade_aluno" }) }),
                            structure:
                              [
                                { name: " ", field: "isProspect", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatProspect },
                                { name: "<input id='selecionaTodosAtividadesAlunos' style='display:none'/>", field: "selecionadoAtividadesAlunos", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividadesAlunos },
                                //{ name: "Código", field: "cd_aluno", width: "15%", minwidth: "10%" },
                                { name: "Nome", field: "no_pessoa", width: "35%", minwidth: "10%" },
                                //{ name: "Escola", field: "dc_reduzido_pessoa_escola", width: "35%", minwidth: "10%" },
                                { name: "Participou", field: "ind_participacao", width: "20%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParticipacao },
                                { name: "Obs.", field: "tx_obs_atividade_aluno", width: "50%", minwidth: "10%" }
                              ],
                            noDataMessage: msgNotRegEnc,
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "34", "68", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    /*page step to be displayed*/
                                    maxPageStep: 5,
                                    /*position of the pagination bar*/
                                    position: "button",
                                    plugins: { nestedSorting: true }
                                }
                            }
                        }, "gridAtividadesAlunos");;
                        gridAtividadesAlunos.canSort = function (col) { return Math.abs(col) != 1; };
                        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                        gridAtividadesAlunos.pagination.plugin._paginator.plugin.connect(gridAtividadesAlunos.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridAtividadesAlunos, 'cd_atividade_aluno', 'selecionaTodosAtividadesAlunos');
                        });

                        gridAtividadesAlunos.startup();
            	        gridAtividadesAlunos.itensSelecionados = [];
                        gridAtividadesAlunos.on("RowDblClick", function (evt) {
                            try{
                                dijit.byId("dialogAluno").show();
                                var idx = evt.rowIndex,
                                   item = this.getItem(idx),
                                   store = this.store;
                                document.getElementById("cd_aluno").value = item.cd_aluno,
                                document.getElementById("aluno").value = item.no_pessoa;
                                dijit.byId("ckParticipouAluno").set("value", item.ind_participacao);
                                document.getElementById("descObs").value = item.tx_obs_atividade_aluno;
                                document.getElementById('divCancelarAluno').style.display = "";
                                document.getElementById('divLimparAluno').style.display = "none";
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAtividadesAlunos"));
                            }
                        });

                        var acaoMarcaParticipou = new MenuItem({
                            label: "Participou",
                            onClick: function () {
                                ParticipouNao(true);
                            }
                        });

                        var acaoMarcaNaoParticipou = new MenuItem({
                            label: "Não Participou",
                            onClick: function () {
                                ParticipouNao(false);
                            }
                        });

                        if (hasValue(dijit.byId("acoesRelacionadasAtividadesAlunos"))) {
                            dijit.byId("acoesRelacionadasAtividadesAlunos").destroy();
                            $('linkAcoesAtividadesAlunos').attr('id', 'gridAtividadesAlunos').appendTo('#contentAtividadesAlunos');
                        }
                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasAtividadesAlunos",
                            dropDown: menu,
                            id: "acoesRelacionadasAtividadesAlunos"
                        });
                        menu.addChild(acaoMarcaParticipou);
                        menu.addChild(acaoMarcaNaoParticipou);
                        if (!eval(parametros['lancamento']))
                            menu.addChild(acaoRemover);
                        dom.byId("linkAcoesAtividadesAlunos").appendChild(button.domNode);

                        //fim da grade

                        dijit.byId("cbProdutos").on("change", function (e) {
                            if (hasValue(e)) {
                                loadCursoByProduto(e);
                            } else {                                
                                dijit.byId("cbProdutos").reset();
                                    dijit.byId("cbCursos").reset();

                                    if (hasValue(dojo.byId('cbCursos').value, true))
                                        dijit.byId("cbCursos").reset();
                                    if (hasValue(dijit.byId('cbCursos')) && hasValue(dijit.byId('cbCursos').store))
                                        dijit.byId('cbCursos').setStore(dijit.byId('cbCursos').store, [0]);
                                    dijit.byId('cbCursos').reset();
                                    dijit.byId("cbCursos").set('disabled', true);
                            }

                        });

                        dijit.byId("hrInicial").on("change", function (hrIni) {
                            try{
                                if (!validarHoraView('hrInicial', 'hrFinal', 'apresentadorMensagemAtividadeExtra')) {
                                    dijit.byId('hrInicial').set('value', null);
                                    return;
                                }

                                var data = hasValue(dojo.byId('dataAtividade').value) ? dojo.byId('dataAtividade').value : 0;
                                var horaFinal = hasValue(dojo.byId('hrFinal').value) ? dojo.byId('hrFinal').value : 0;
                                var horaInicial = hasValue(hrIni) ? hrIni : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("hrFinal").on("change", function (hrFinal) {
                            try{
                                if (!validarHoraView('hrInicial', 'hrFinal', 'apresentadorMensagemAtividadeExtra')) {
                                    dijit.byId('hrFinal').set('value', null);
                                    return;
                                }

                                var data = hasValue(dojo.byId('dataAtividade').value) ? dojo.byId('dataAtividade').value : 0;
                                var horaInicial = hasValue(dojo.byId("hrInicial").value) ? dojo.byId("hrInicial").value : 0;
                                var horaFinal = hasValue(hrFinal) ? hrFinal : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("dataAtividade").on("change", function (dta) {
                            try{
                                var horaInicial = hasValue(dojo.byId("hrInicial").value) ? dojo.byId("hrInicial").value : 0;
                                var horaFinal = hasValue(dojo.byId('hrFinal').value) ? dojo.byId('hrFinal').value : 0;
                                var data = hasValue(dta) ? dta : 0;
                                manipulaDropDownSala(horaInicial, horaFinal, data);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("cbSalas").on("click", function (e) {
                            if (dijit.byId("cbSalas").store.data == null) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Deverão ser informados primeiramente a Data e horários inicial e final para a escolha de salas');
                                apresentaMensagem('apresentadorMensagemAtividadeExtra', mensagensWeb);
                            }
                        });

                        dijit.byId("cbSalas").on("change", function (e) {
                            if (hasValue(e)) {
                                limparGrid();
                                
                                verificaSalaOnlineOnchangeSala(ObjectStore, Memory);
                                
                                popularGridEscolaAtividade();
                                dijit.byId("gridEscolas")._refresh();
                                dijit.byId("tagEscola").set("open", false);
                            } else {
                                dojo.byId("tagEscola").style.display = "none";
                                dijit.byId("gridEscolas")._refresh();
                                dijit.byId("tagEscola").set("open", false);

                            }
                            
                        });



                        dijit.byId("cbTiposAtividades"). on("change", function(e) {
                                if (hasValue(e)) {
                                    if (e === EnumTipoAtividade.AULAEXPERIMENTAL) {
                                        dijit.byId("tagAlunos").setTitle("Alunos/Clientes/Prospects");
                                        
                                        require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                                            domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', '');
                                        });

                                        if (hasValue(dijit.byId("cbSalas").value)) {
                                            verificaSalaOnlineOnchangeTipoSala(ObjectStore, Memory);
                                        }
                                    } else {
                                        dijit.byId("tagAlunos").setTitle("Alunos/Clientes");
                                        require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                                            domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', 'none');
                                        });
                                    }
                                } else {
                                    dijit.byId("tagAlunos").setTitle("Alunos/Clientes");
                                    require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                                        domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', 'none');
                                    });
                                }
                        });

                        //Recorrencia
                        dijit.byId("id_data_termino_1").on("change",
                            function (e) {
                                if (hasValue(e)) {
                                    dijit.byId("dt_limite").set("required", true);
                                    
                                } else
                                {
                                    dijit.byId("dt_limite").set("required", false);
                                }
                                getStatusRecorrencia();
                            });

                        dijit.byId("id_data_termino_2").on("change",
                            function (e) {
                                if (hasValue(e)) {
                                    dijit.byId("nm_eventos").set("required", true);
                                   
                                } else {
                                    dijit.byId("nm_eventos").set("required", false);
                                }
                                getStatusRecorrencia();
                            });

                        dijit.byId("nm_frequencia").on("change",
                            function(e) {
                                if (hasValue(e)) {
                                    dijit.byId("id_tipo_recorrencia").set("value", e);
                                }
                                getStatusRecorrencia();
                        });

                        dijit.byId("id_tipo_recorrencia").on("change",
                            function(e) {
                                if (hasValue(e)) {
                                    dijit.byId("nm_frequencia").set("value", e);
                                }
                                getStatusRecorrencia();
                            });

                        dijit.byId("nm_eventos").on("change",
                            function (e) {
                                getStatusRecorrencia();
                            });
                        dijit.byId("dt_limite").on("change",
                            function (e) {
                                    getStatusRecorrencia();
                            });
                        
                       

                        //fim Recorrencia

                        dijit.byId("tagAlunos").on("show", function (e) {
                            dijit.byId("gridAtividadesAlunos").update();
                            dijit.byId("contentAtividadesAlunos").resize();
                        });

                        dijit.byId("tagEscola").on("show", function (e) {
                            dijit.byId("gridEscolas").update();
                            dijit.byId("panelEscolasFK").resize();

                            pesquisarEscolasVinculadasUsuarioAtividade();
                            popularGridEscolaAtividade();
                            dojo.byId('abriuEscola').value = true;
                           
                        });

                        if (hasValue(dijit.byId("menuManual"))) {
                            if (hasValue(dijit.byId("menuManual"))) {
                                dijit.byId("menuManual").on("click",
                                    function(e) {
                                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323056',
                                            '765px',
                                            '771px');
                                    });
                            }
                        }
                        adicionarAtalhoPesquisa(['cbResponsavel', 'cbCurso', 'cbTipoAtividade', 'cbProduto', 'dtaIni', 'hhIni', 'dtaFim', 'hhFim'], 'pesquisarAtividadeExtra', ready);

                        if (desabilitaCamposLancamento) {
                            setTimeout(function (){
                                dijit.byId('novaAtividadeExtra').set('disabled', true);

                                dijit.byId('cbTiposAtividades').set('disabled', true);
                                dijit.byId('nroVagas').set('disabled', true);
                                dijit.byId('hrInicial').set('disabled', true);
                                dijit.byId('hrFinal').set('disabled', true);
                                dijit.byId('dataAtividade').set('disabled', true);
                                dijit.byId('cbProdutos').set('disabled', true);
                                dijit.byId('cbResponsaveis').set('disabled', true);
                                dijit.byId('ckCarga').set('disabled', true);
                                dijit.byId('ckPagar').set('disabled', true);
                                dijit.byId('ckPortal').set('disabled', true);
                                dijit.byId('hrLimite').set('disabled', true);
                                dijit.byId('cbSalas').set('disabled', true);
                                dijit.byId('txObs').set('disabled', true);
                                dijit.byId('cbCursos').set('disabled', true);
                            }, 1000);
                        }
                        showCarregando();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (exception) {
                    showCarregando();

                    var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                idProperty: "cd_atividade_extra"
                            }
                        ), Memory({ idProperty: "cd_atividade_extra" }));

                    var gridAtividadeExtra = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        structure: [
                            { name: "<input id='selecionaTodosAtividadeExtra' style='display:none'/>", field: "selecionadoAtividadeExtra", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividadeExtra },
                            { name: "Data", field: "dta_atividade_extra", width: "10%", styles: "text-align: left; min-width:75px; max-width:75px;" },
                            { name: "Hr. Inicial", field: "hh_inicial", width: "10%", styles: "text-align: center;" },
                            { name: "Hr. Final", field: "hh_final", width: "10%", styles: "text-align: center;" },
                            { name: "Tipo Atividade", field: "no_tipo_atividade_extra", width: "10%", styles: "text-align: left;" },
                            { name: "Produto", field: "no_produto", width: "10%", styles: "text-align: left;" },
                            { name: "Curso", field: "no_curso", width: "10%", styles: "text-align: left;" },
                            { name: "Responsável", field: "no_responsavel", width: "20%", styles: "text-align: left;" },
                            { name: "Vagas", field: "nm_vagas", width: "10%", styles: "text-align: center;" },
                            { name: "Alunos", field: "nm_alunos", width: "10%", styles: "text-align: center;" }
                        ],
                        noDataMessage: msgNotRegEnc,
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                maxPageStep: 4,
                                position: "button",
                                plugins: { nestedSorting: true }
                            }
                        }
                    }, "gridAtividadeExtra");
                    gridAtividadeExtra.startup();

                    new Button({
                        label: "Novo",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF'
                    }, "novaAtividadeExtra");

                    //----Monta o botão de Relatório----
                    new Button({
                        label: getNomeLabelRelatorio(),
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage'
                    }, "relatorioAtividadeExtra");

                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIconSearchSGF'                     
                    }, "pesquisarAtividadeExtra");
                    decreaseBtn(document.getElementById("pesquisarAtividadeExtra"), '32px');

                    // botões de pesquisa de aluno
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesAluno");

                    new Button({
                        label: "limpar", iconClass: '', type: "reset"
                    }, "limparAlunoPesc");

                    if (hasValue(document.getElementById("limparAlunoPesc"))) {
                        document.getElementById("limparAlunoPesc").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPesc").parentNode.style.width = '40px';
                    }

                    //Altera o tamanho dos botões
                    var buttonFkArray = ['pesAluno'];
                    diminuirBotoes(buttonFkArray);

                });
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
// Fim da criação das grades

function getStatusRecorrencia() {
    var statusRecorrencia = "";

    statusRecorrencia = getTipoRecorrencia() + ", " + getDataTermino();
    require(["dojo/dom"], function (dom) {
        dom.byId("statusEvento").innerHTML = statusRecorrencia;
    });
}

function getTipoRecorrencia() {
    if (hasValue(dijit.byId("id_tipo_recorrencia").value)) {
        if (dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.DIARIAMENTE.toString()) {
            return "Diariamente";
        }else if (dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.SEMANALMENTE.toString()) {
            return "Semanalmente";
        } else if (dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.QUINZENALMENTE.toString()) {
            return "Quinzenalmente";
        } else if (dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.MENSALMENTE.toString()) {
            return "Mensalmente";
        } else if (dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.ANUALMENTE.toString()) {
            return "Anualmente";
        } else {
            return "(*Preecher Recorrêndia)";
        }
    } else {
        return "(*Preecher Recorrêndia)";
    }
}


function getDataTermino() {
    if (dijit.byId("id_data_termino_1").checked === true) {
        if (dijit.byId("dt_limite").value != 'Invalid Date' &&
            dijit.byId("dt_limite").value != undefined) {
            return "até " + dojo.byId("dt_limite").value;
        } else {
            return "(*Preecher Término)";
        }
    }

    if (dijit.byId("id_data_termino_2").checked === true) {
        if (hasValue(dijit.byId("nm_eventos").value)) {
            return "após " + dojo.byId("nm_eventos").value + " eventos";
        } else {
            return "(*Preecher Número de eventos)";
        }
    }
    
}


function verificaAulaExperimental() {
    try {
        if (hasValue(dijit.byId("cbTiposAtividades").value)) {
            if (dijit.byId("cbTiposAtividades").value === EnumTipoAtividade.AULAEXPERIMENTAL) {
                dijit.byId("tagAlunos").setTitle("Alunos/Clientes/Prospects");

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', '');
                });
            } else {
                dijit.byId("tagAlunos").setTitle("Alunos/Clientes");
                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', 'none');
                });
            }
        } else {
            dijit.byId("tagAlunos").setTitle("Alunos/Clientes");
            require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                domStyle.set(registry.byId("incluirProspectFK").domNode, 'display', 'none');
            });
        }
    } catch (e) {
        postGerarLog(e);
    } 
}

function manipulaDropDownSala(horaInicial, horaFinal, data) {
    try{
        if (horaInicial == 0 || horaFinal == 0 || data == 0) {
            dijit.byId("cbSalas").set("disabled", true);
            dijit.byId("cbSalas").reset();
            dijit.byId("cbSalas").store.data = null;
        }
        else {
            dijit.byId("cbSalas").reset();
            var parametros = getParamterosURL();
            if (!eval(parametros['lancamento']))
                dijit.byId("cbSalas").set("disabled", false);
            populaSala("cbSalas", horaInicial, horaFinal, data);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


//#region  eventos para a aba de escola
function getEmpresasGridEscolasAtividadeExtra() {
    var listaEmpresasGrid = "";

    if (hasValue(dijit.byId("gridEscolas").store.objectStore.data))
        $.each(dijit.byId("gridEscolas").store.objectStore.data, function (index, value) {
            listaEmpresasGrid += value.cd_escola + ",";
        });
    return listaEmpresasGrid;
}


function pesquisarEscolasFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory",
            "dojo/ready"
        ],
        function(JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function() {
                try {
                    var listaEmpresasGrid = getEmpresasGridEscolasAtividadeExtra();
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() +
                                "/api/empresa/findAllEmpresasByUsuarioPag?cdEmpresas=" +
                                listaEmpresasGrid +
                                "&nome=" +
                                dojo.byId("_nomePessoaFK").value +
                                "&fantasia=" +
                                dojo.byId("_apelido").value +
                                "&cnpj=" +
                                dojo.byId("CnpjCpf").value +
                                "&inicio=" +
                                document.getElementById("inicioPessoaFK").checked,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }),
                        Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoa");
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        });

}


function pesquisarEscolasVinculadasUsuarioAtividade() {

    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/empresa/findQuantidadeEmpresasVinculadasUsuario",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {

            if (data != null && data != undefined && data >= 0) {
                dojo.byId("qtdEscolasVinculadasUsuario").value = data;
            } else {
                dojo.byId("qtdEscolasVinculadasUsuario").value = 0;
            }

            if (dojo.byId("qtdEscolasVinculadasUsuario").value == 0) {
                dojo.byId("tagEscola").style.display = "none";
            } 

        
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemMat', error);
    });

}

function verificaSalaOnline(mostrar) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/verificaSalaOnline?cd_sala=" + dijit.byId("cbSalas").value,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
            try {

                if (data == "true" && mostrar) {
                    dojo.byId("tagEscola").style.display = "";
                    dijit.byId("tagEscola").set("open", false);
                } else {
                    dojo.byId("tagEscola").style.display = "none";
                }

            } catch (e) {
                postGerarLog(e);
            }

        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
}



function verificaSalaOnlineOnchangeSala(ObjectStore, Memory) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/verificaSalaOnline?cd_sala=" + dijit.byId("cbSalas").value,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
            try {

                if (data == "true") {
                    dojo.byId("tagEscola").style.display = "";
                    dijit.byId("tagEscola").set("open", false);

                    if (dojo.byId("tagEscola").style.display === "") {
                        var gridAlunos = dijit.byId("gridAtividadesAlunos");
                        gridAlunos.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                        gridAlunos.update();

                    }
                } else {

                    if (dijit.byId("cbTiposAtividades").value === EnumTipoAtividade.AULAEXPERIMENTAL) {
                        dijit.byId("cbSalas").reset();
                        caixaDialogo(DIALOGO_AVISO, "Para o tipo de atividade extra \"Aula Experimental\", apenas salas online podem ser selecionadas.", null);
                    }
                    dojo.byId("tagEscola").style.display = "none";
                }

            } catch (e) {
                postGerarLog(e);
            }

        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
}


function verificaSalaOnlineOnchangeTipoSala(ObjectStore, Memory) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/verificaSalaOnline?cd_sala=" + dijit.byId("cbSalas").value,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
            try {

                if (data != "true")  {

                    if (dijit.byId("cbTiposAtividades").value === EnumTipoAtividade.AULAEXPERIMENTAL) {
                        dijit.byId("cbSalas").reset();
                        caixaDialogo(DIALOGO_AVISO, "Para o tipo de atividade extra \"Aula Experimental\", apenas salas online podem ser selecionadas.", null);
                    }
                }

            } catch (e) {
                postGerarLog(e);
            }

        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
}

function abrirPessoaFK(isPesquisa) {
    dojo.ready(function () {
        try {
            dijit.byId("fkPessoaPesq").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaPesq").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaEscolaFK() {
    try {
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        if (hasValue(gridEscolas)) {
            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridEscolas.update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_escola');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_escola", {
                        cd_atividade_escola: 0,
                        cd_atividade_extra: dojo.byId("cd_atividade_extra").value,
                        cd_escola: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function popularGridEscolaAtividade() {
    try {
        var atividade = hasValue(dojo.byId('cd_atividade_extra').value) ? dojo.byId('cd_atividade_extra').value : 0;
        var gridEscolas = dijit.byId('gridEscolas');
        var popular = !eval(dojo.byId('abriuEscola').value);
        if (gridEscolas != null && gridEscolas.store != null && gridEscolas.store.objectStore.data.length == 0 && popular) {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getAtividadeEscolatWithAtividade?cd_atividade_extra=" + atividade,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagemAtividadeExtra', data);
                    var empresas = data.retorno;
                    gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                    gridEscolas.update();
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarStringCdsEscolas() {
    try {

        var gridEscolas = dijit.byId('gridEscolas');
        var listaEscolas = "";
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null &&
                gridEscolas.store.objectStore != null &&
                gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = "";
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {
                    if (listaEscolas === "") {
                        listaEscolas = escolas[i].cd_escola;
                    } else {
                        listaEscolas = listaEscolas + "|" + escolas[i].cd_escola;
                    }
                    
                }
            }
        }
        return listaEscolas;

    } catch (e) {

    }
}

function montarEscolasGridAtividade() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        var listaEscolas = [];
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null && gridEscolas.store.objectStore != null && gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = [];
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {
                    listaEscolas.push({
                        cd_atividade_escola: escolas[i].cd_atividade_escola,
                        cd_atividade_extra: escolas[i].cd_atividade_extra,
                        cd_escola: escolas[i].cd_escola,
                        dc_reduzido_pessoa: escolas[i].dc_reduzido_pessoa
                    });
                }
            }
        }
        return listaEscolas;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montarObjetoAtividadeExtra(item) {
    try {
        
        var objeto =
            {
                cd_atividade_extra: item.cd_atividade_extra,
                cd_cursos: item.cd_cursos,
                cd_funcionario: item.cd_funcionario,
                cd_produto: item.cd_produto,
                cd_tipo_atividade_extra: item.cd_tipo_atividade_extra,
                dta_atividade_extra: item.dta_atividade_extra,
                hh_final: item.hh_final,
                hh_inicial: item.hh_inicial,
                ind_carga_horaria: item.ind_carga_horaria,
                ind_pagar_professor: item.ind_pagar_professor,
                id_calendario_academico: item.id_calendario_academico,
                hr_limite_academico: item.hr_limite_academico,
                nm_vagas: item.nm_vagas,
                tx_obs_atividade: item.tx_obs_atividade,
                cd_usuario_atendente: item.cd_usuario_atendente,
                cd_sala: item.cd_sala,
                no_tipo_atividade_extra: item.no_tipo_atividade_extra,
                no_curso: item.no_curso,
                no_produto: item.no_produto,
                no_responsavel: item.no_responsavel,
                atividadesAluno: item.atividadeAluno,
                selecionadoAtividadeExtra: item.itemSelecionado,
                no_usuario: item.no_usuario,
                cd_pessoa_escola: item.cd_pessoa_escola
                
            };
        return objeto;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function AtividadeExtraPesquisa(item) {
    try{
        if (item != null) {
            this.cd_pessoa_escola = item.cd_pessoa_escola;
            this.cd_atividade_extra = item.cd_atividade_extra;
            this.data = item.dta_atividade_extra;
            this.hrInicial = item.hh_inicial;
            this.hrFinal = item.hh_final;
            this.cd_produto = item.cd_produto;
            this.cd_curso = item.cd_curso;
            this.cd_funcionario = item.cd_funcionario;
            this.cd_aluno = item.cd_aluno;
            this.cd_tipo_ativiade_extra = item.cd_tipo_atividade_extra;
            this.cd_sala = item.cd_sala;
        } else {
            this.cd_pessoa_escola = 0;
            this.cd_atividade_extra = null;
            this.data = "";
            this.hrInicial = "";
            this.hrFinal = "";
            this.cd_produto = 0;
            this.cd_curso = 0;
            this.cd_funcionario = 0;
            this.cd_aluno = 0;
            this.cd_tipo_ativiade_extra = 0;
            this.cd_sala = 0;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region Persistêcia da atividade extra

//Método de pesquisa

function pesquisarAtividadeExtra(limparItens,lancada) {
    if (!validarHoraView('hhIni', 'hhFim', 'apresentadorMensagem'))
        return false;

    if (!validarDatasIniFim('dtaIni','dtaFim' ,'apresentadorMensagem', dojo.date))
        return false;

    var variaveisPesquisa = new VariaveisPesquisa(lancada);
    variaveisPesquisa.hroFim = variaveisPesquisa.hroFim == '' ? null : variaveisPesquisa.hroFim;
    variaveisPesquisa.hroIni = variaveisPesquisa.hroIni == '' ? null : variaveisPesquisa.hroIni;

    if (!dijit.byId('hhIni').validate() || !dijit.byId('hhFim').validate() ||
       !dijit.byId('dtaIni').validate() || !dijit.byId('dtaFim').validate())
        return false;
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getAtividadeExtraSearch?dataIni=" + variaveisPesquisa.dtaIni + "&dataFim=" + variaveisPesquisa.dtaFim + "&hrInicial=" + variaveisPesquisa.hroIni + "&hrFinal=" + variaveisPesquisa.hroFim + "&tipoAtividade=" + variaveisPesquisa.tipoAtividade + "&curso=" + variaveisPesquisa.curso + "&responsavel=" + variaveisPesquisa.responsavel + "&produto=" + variaveisPesquisa.produto + "&aluno=" + variaveisPesquisa.aluno + "&lancada=" + variaveisPesquisa.lancada + "&cd_escola_combo=0" ,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_atividade_extra"
                }
                ), Memory({ idProperty: "cd_atividade_extra" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAtividadeExtra = dijit.byId("gridAtividadeExtra");
            if (limparItens) {
                gridAtividadeExtra.itensSelecionados = [];
            }
            gridAtividadeExtra.noDataMessage = msgNotRegEnc;
            gridAtividadeExtra.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function VariaveisPesquisa(vlancada) {
    try{
        if (hasValue(dojo.byId("dtaIni").value)) {
            this.dtaIni = dojo.byId("dtaIni").value;
            // this.dtaIni = dateFormat(dtaIni.value, 'dd/mm/yyyy');
        } else {
            this.dtaIni = minDate;
        }
        if (hasValue(dojo.byId("dtaFim").value)) {
            this.dtaFim = dojo.byId("dtaFim").value;
            //  this.dtaFim = dateFormat(dtaFim.value, 'dd/mm/yyyy');
        } else {
            this.dtaFim = maxDate;
        }
        if (hasValue(dojo.byId("hhIni").value))
            this.hroIni = dojo.byId("hhIni").value;
        else
            this.hroIni = '';
        if (hasValue(dojo.byId("hhFim").value))
            this.hroFim = dojo.byId("hhFim").value;
        else
            this.hroFim = '';

        this.tipoAtividade = hasValue(dijit.byId("cbTipoAtividade").value) ? dijit.byId("cbTipoAtividade").value : null;
        this.curso = hasValue(dijit.byId("cbCurso").value) ? dijit.byId("cbCurso").value : null;
        this.responsavel = hasValue(dijit.byId("cbResponsavel").cd_pessoa) ? dijit.byId("cbResponsavel").cd_pessoa : (hasValue(dijit.byId("cbResponsavel").value) ? dijit.byId("cbResponsavel").value : null);
        this.produto = hasValue(dijit.byId("cbProduto").value) ? dijit.byId("cbProduto").value : null;
        this.aluno = hasValue(dojo.byId("cdAlunoPesq").value) ? dojo.byId("cdAlunoPesq").value : null;
        this.lancada = hasValue(dijit.byId('cbLancada_1').value, true) ? dijit.byId('cbLancada_1').value : vlancada;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//IncluirItemServico
function IncluirAtividadeExtra() {
    require([
     "dojo/date",
     "dojo/ready"
    ], function (date, ready) {
        ready(function () {
            try {
                apresentaMensagem('apresentadorMensagem', null);
                apresentaMensagem('apresentadorMensagemAtividadeExtra', null);
                if (hasValue(dojo.byId('cdUsuario').value)) {
                    var cdUsuario = dojo.byId('cdUsuario').value;
                } else {
                    caixaDialogo(DIALOGO_AVISO, 'Usúario não identificado.', null);
                    return false;
                }
                var atividadeAluno = montarAlunoAtividadeExtra();
                var atividadeProspect = montarProspectAtividadeExtra();
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

                var dataMin = new Date(1899, 12, 01);
                var dataMax = new Date(2079, 05, 06);
                if (date.compare(dataMin, dataAtividade) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMin);
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                    return false;
                } else if (date.compare(dataAtividade, dataMax) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMax);
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                    return false;
                }

                if (!dijit.byId("formAtividadeExtra").validate()) return false;
                require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                    showCarregando();
                    xhr.post(Endereco() + "/api/coordenacao/postIncluirAtividadeExtra", {
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        handleAs: "json",
                        data: ref.toJson({
                            cd_cursos: dijit.byId("cbCursos").value,
                            cd_funcionario: dijit.byId("cbResponsaveis").value,
                            cd_produto: dijit.byId("cbProdutos").value,
                            cd_tipo_atividade_extra: dijit.byId("cbTiposAtividades").value,
                            dt_atividade_extra: dataAtividade,
                            hh_final: dom.byId("hrFinal").value,
                            hh_inicial: dom.byId("hrInicial").value,
                            ind_carga_horaria: domAttr.get("ckCarga", "checked"),
                            ind_pagar_professor: domAttr.get("ckPagar", "checked"),
                            id_calendario_academico: domAttr.get("ckPortal", "checked"),
                            hr_limite_academico: dom.byId("hrLimite").value,
                            nm_vagas: dojo.number.parse(dom.byId("nroVagas").value),
                            tx_obs_atividade: dom.byId("txObs").value,
                            cd_usuario_atendente: cdUsuario,
                            no_usuario: hasValue(dojo.byId('txAtendente').value) ? dojo.byId('txAtendente').value : "",
                            cd_sala: dijit.byId("cbSalas").value,
                            no_tipo_atividade_extra: dojo.byId("cbTiposAtividades").value,
                            no_curso: dojo.byId("cbCursos").value,
                            no_produto: dojo.byId("cbProdutos").value,
                            no_responsavel: dojo.byId("cbResponsaveis").value,
                            atividadesAluno: atividadeAluno,
                            atividadesProspect: atividadeProspect,
                            AtividadeEscolaAtividade: (dojo.byId("tagEscola").style.display === "none")? new Array() : montarEscolasGridAtividade(),
                            hasClickEscola:dojo.byId('abriuEscola').value
                        })
                    }).then(function (data) {
                        try {
                            showCarregando();
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridAtividadeExtra';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadAtividadeExtra").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                insertObjSort(grid.itensSelecionados, "cd_atividade_extra", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionadoAtividadeExtra', 'cd_atividade_extra', 'selecionaTodosAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_atividade_extra");
                            } else
                                apresentaMensagem('apresentadorMensagemAtividadeExtra', data);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
                    });
                });
            }
            catch (er) {
                postGerarLog(er);
            }
        });
    });
}

function GerarRecorrenciaAtividadeExtra(itemSelecionado) {
    require([
        "dojo/date",
        "dojo/ready"
    ], function (date, ready) {
        ready(function () {
            try {
                apresentaMensagem('apresentadorMensagem', null);
                apresentaMensagem('apresentadorMensagemAtividadeExtra', null);
                apresentaMensagem('apresentadorMensagemRecorrencia', null);

                var atividadeSelecionada = itemSelecionado;
                atividadeSelecionada.ColunasRelatorio = null;
                atividadeSelecionada.atividadesAluno = null;
                var dataAtividadeExtra = hasValue(atividadeSelecionada.dta_atividade_extra) ? dojo.date.locale.parse(atividadeSelecionada.dta_atividade_extra, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                var dtLimite = hasValue(dojo.byId("dt_limite").value) ? dojo.date.locale.parse(dojo.byId("dt_limite").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                if (dijit.byId("id_tipo_recorrencia").validate() === false) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTipoRecorrenciaNulo);
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                } else if (dijit.byId("nm_frequencia").validate() === false) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNumeroFrequenciaNulo);
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                } else if (dijit.byId("dt_limite").validate() === false) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataLimiteNulaItemAteMarcado);
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                }  else if (dijit.byId("nm_eventos").validate() === false) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNumeroEventosNuloItemAposMarcado);
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                }
                else if (dijit.byId("id_data_termino_1").checked === true &&
                    dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.SEMANALMENTE.toString()
                    && date.compare(dojo.date.add(dataAtividadeExtra, "day", 7), dtLimite) > 0) {
                    var dtSemanal = dojo.date.locale.format(dojo.date.add(dataAtividadeExtra, "day", 7),
                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataLimiteMenorTipoRecorrenciaSemanal + dtSemanal + ").");
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                } else if (dijit.byId("id_data_termino_1").checked === true &&
                     dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.QUINZENALMENTE.toString()
                    && date.compare(dojo.date.add(dataAtividadeExtra, "day", 15), dtLimite) > 0) {
                    var dtQuinzenal = dojo.date.locale.format(dojo.date.add(dataAtividadeExtra, "day", 15),
                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataLimiteMenorTipoRecorrenciaQuinzenal + dtQuinzenal + ").");
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                }
                else if (dijit.byId("id_data_termino_1").checked === true &&
                    dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.MENSALMENTE.toString()
                    && date.compare(dojo.date.add(dataAtividadeExtra, "month", 1), dtLimite) > 0) {
                    var dtMensal = dojo.date.locale.format(dojo.date.add(dataAtividadeExtra, "month", 1),
                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataLimiteMenorTipoRecorrenciaMensal + dtMensal + ").");
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                }
                else if (dijit.byId("id_data_termino_1").checked === true &&
                    dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.ANUALMENTE.toString()
                    && date.compare(dojo.date.add(dataAtividadeExtra, "year", 1), dtLimite) > 0) {
                    var dtAnual = dojo.date.locale.format(dojo.date.add(dataAtividadeExtra, "year", 1),
                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataLimiteMenorTipoRecorrenciaAnual + dtAnual + ").");
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                }
                else if (dijit.byId("id_data_termino_1").checked === true &&
                    dijit.byId("id_tipo_recorrencia").value === EnumTipoRecorrencia.DIARIAMENTE.toString() &&
                    date.compare(dataAtividadeExtra, dtLimite) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                        msgErroDataTerminoRecorrenciaMenorDataAtividadeExtra +
                        atividadeSelecionada.dta_atividade_extra +
                        ").");
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                    return false;
                } 
                            
                var atividadeExtraRecorrencia = montarAtividadeExtraForRecorrencia(atividadeSelecionada);

                require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                    showCarregando();
                    xhr.post(Endereco() + "/api/coordenacao/postGerarRecorrenciaAtividadeExtra", {
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        handleAs: "json",
                        data: ref.toJson(atividadeExtraRecorrencia)
                    }).then(function (data) {
                        try {
                            showCarregando();
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridAtividadeExtra';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId('dialogGerarRecorrencia').hide();

                                var parametros = getParamterosURL();
                                var vlLanc = eval(parametros['lancamento']) ? 0 : 2;
                                var lanc = hasValue(dijit.byId('cbLancada_1').value, true) ? dijit.byId('cbLancada_1').value : vlancada;
                                buscarTodosItens(dijit.byId('gridAtividadeExtra'), 'todosItensAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra']); pesquisarAtividadeExtra(true, 2);

                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                } else {
                                    grid.itensSelecionados = dijit.byId("gridAtividadeExtra").store.objectStore.get(grid.itensSelecionados[0].cd_atividade_extra);
                                }
                                
                                
                                //insertObjSort(grid.itensSelecionados, "cd_atividade_extra", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionadoAtividadeExtra', 'cd_atividade_extra', 'selecionaTodosAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                //setGridPagination(grid, itemAlterado, "cd_atividade_extra");
                            } else
                                apresentaMensagem('apresentadorMensagemRecorrencia', data);
                        }
                        catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                    function (error) {
                        hideCarregando();
                        apresentaMensagem('apresentadorMensagemRecorrencia', error.response.data);
                    });
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function AlteraAtividadeExtra() {
    require([
        "dojo/date",
        "dojo/ready"
    ], function (date, ready) {
        ready(function () {
            try {
                var escola = jQuery.parseJSON(Escola());
                apresentaMensagem('apresentadorMensagem', null);
                apresentaMensagem('apresentadorMensagemItemServico', null);
                if (hasValue(dojo.byId('cdUsuario').value)) {
                    var cdUsuario = dojo.byId('cdUsuario').value;
                } else {
                    caixaDialogo(DIALOGO_AVISO, 'Usuário não identificado.', null);
                    return false;
                }
                var codAtividadeExtra = dojo.byId("cd_atividade_extra").value;
                var dataAtividade = hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                if (!dijit.byId("formAtividadeExtra").validate()) return false;
                var atividadeAluno = montarAlunoAtividadeExtra();
                var atividadeProspect = montarProspectAtividadeExtra();

                var dataMin = new Date(1899, 12, 01);
                var dataMax = new Date(2079, 05, 06);
                if (date.compare(dataMin, dataAtividade) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMin);
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                    return false;
                } else if (date.compare(dataAtividade, dataMax) > 0) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtaMax);
                    apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
                    return false;
                }

                require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                    showCarregando();
                    xhr.post(Endereco() + "/api/coordenacao/postAlterarAtividadeExtra", {
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        handleAs: "json",
                        data: ref.toJson({
                            cd_atividade_extra: codAtividadeExtra,
                            cd_cursos: dijit.byId("cbCursos").value,
                            cd_funcionario: dijit.byId("cbResponsaveis").value,
                            cd_produto: dijit.byId("cbProdutos").value,
                            cd_tipo_atividade_extra: dijit.byId("cbTiposAtividades").value,
                            dt_atividade_extra: dataAtividade,
                            hh_final: dom.byId("hrFinal").value,
                            hh_inicial: dom.byId("hrInicial").value,
                            ind_carga_horaria: domAttr.get("ckCarga", "checked"),
                            ind_pagar_professor: domAttr.get("ckPagar", "checked"),
                            nm_vagas: dojo.number.parse(dom.byId("nroVagas").value),
                            id_calendario_academico: domAttr.get("ckPortal", "checked"),
                            hr_limite_academico: dom.byId("hrLimite").value,
                            tx_obs_atividade: dom.byId("txObs").value,
                            cd_usuario_atendente: cdUsuario,
                            cd_sala: dijit.byId("cbSalas").value,
                            no_tipo_atividade_extra: dojo.byId("cbTiposAtividades").value,
                            no_curso: dojo.byId("cbCursos").value,
                            no_produto: dojo.byId("cbProdutos").value,
                            no_responsavel: dojo.byId("cbResponsaveis").value,
                            no_usuario: hasValue(dojo.byId('txAtendente').value) ? dojo.byId('txAtendente').value : "",
                            cd_escola: escola,
                            atividadesAluno: atividadeAluno,
                            atividadesProspect: atividadeProspect,
                            AtividadeEscolaAtividade: (dojo.byId("tagEscola").style.display === "none")? new Array() : montarEscolasGridAtividade(),
                            hasClickEscola: dojo.byId('abriuEscola').value
                        })
                    }).then(function (data) {
                        try {
                            showCarregando();
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridAtividadeExtra';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadAtividadeExtra").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                removeObjSort(grid.itensSelecionados, "cd_atividade_extra", dom.byId("cd_atividade_extra").value);
                                insertObjSort(grid.itensSelecionados, "cd_atividade_extra", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionadoAtividadeExtra', 'cd_atividade_extra', 'selecionaTodosAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_atividade_extra");
                            } else
                                apresentaMensagem('apresentadorMensagemAtividadeExtra', data);
                        }
                        catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                    function (error) {
                        hideCarregando();
                        apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
                    });
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

// Deletar Atividade Extra
function DeletarAtividadeExtra(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_atividade_extra').value != 0 && dojo.byId('cd_pessoa_escola').value != 0)
                itensSelecionados = [{
                    cd_atividade_extra: dom.byId("cd_atividade_extra").value,
                    dt_atividade_extra: hasValue(dojo.byId("dataAtividade").value) ? dojo.date.locale.parse(dojo.byId("dataAtividade").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    cd_pessoa_escola: dom.byId("cd_pessoa_escola").value

                }];
        if ((itensSelecionados[0].cd_pessoa_escola + "") !== dojo.byId("_ES0").value) {
            caixaDialogo(DIALOGO_ERRO,
                "A funcionalidade para deletar atitividades não é permitida para atividades de escolas diferentes da escola logada.<br> O item selecionado é uma atividade de outra escola.",
                null);
        } else {
            xhr.post({
                url: Endereco() + "/api/coordenacao/PostDeleteAtividadesExtras",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                    try {
                        var todos = dojo.byId("todosItensAtividadeExtra");
                        apresentaMensagem('apresentadorMensagem', data);
                        data = jQuery.parseJSON(data).retorno;
                        dijit.byId("cadAtividadeExtra").hide();
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridAtividadeExtra').itensSelecionados, "cd_atividade_extra", itensSelecionados[r].cd_atividade_extra);
                        var parametros = getParamterosURL();
                        var lancada = eval(parametros['lancamento']) ? 0 : 2;
                        pesquisarAtividadeExtra(true, lancada);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarAtividadeExtra").set('disabled', false);
                        dijit.byId("relatorioAtividadeExtra").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    if (!hasValue(dojo.byId("cadAtividadeExtra").style.display))
                        apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        }
        

        
    });
}

function DeletarRecorrencias(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0 || itensSelecionados.length > 1) {
            return false;
        }
            
        xhr.post({
            url: Endereco() + "/api/coordenacao/PostDeleteRecorrencias",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados[0])
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensAtividadeExtra");
                apresentaMensagem('apresentadorMensagem', data);
                data = jQuery.parseJSON(data).retorno;
                dijit.byId('dialogGerarRecorrencia').hide();

                var parametros = getParamterosURL();
                var vlLanc = eval(parametros['lancamento']) ? 0 : 2;
                var lanc = hasValue(dijit.byId('cbLancada_1').value, true) ? dijit.byId('cbLancada_1').value : vlancada;
                buscarTodosItens(dijit.byId('gridAtividadeExtra'), 'todosItensAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra']); pesquisarAtividadeExtra(true, 2);
                
                dijit.byId('gridAtividadeExtra').itensSelecionados[0] = dijit.byId("gridAtividadeExtra").store.objectStore.get(dijit.byId('gridAtividadeExtra').itensSelecionados[0].cd_atividade_extra);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarAtividadeExtra").set('disabled', false);
                dijit.byId("relatorioAtividadeExtra").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
                apresentaMensagem('apresentadorMensagemRecorrencia', error);
        });
    });
}


function EnviarEmailRecorrencias(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0 || itensSelecionados.length > 1) {
            return false;
        }

        xhr.post({
            url: Endereco() + "/api/coordenacao/PostSendEmailRecorrencias",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados[0])
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                if (hasValue(data)) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagemRecorrencia", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, data);
                    apresentaMensagem("apresentadorMensagemRecorrencia", mensagensWeb);
                } else {
                    dijit.byId('dialogGerarRecorrencia').hide();

                    var parametros = getParamterosURL();
                    var vlLanc = eval(parametros['lancamento']) ? 0 : 2;
                    var lanc = hasValue(dijit.byId('cbLancada_1').value, true) ? dijit.byId('cbLancada_1').value : vlancada;
                    buscarTodosItens(dijit.byId('gridAtividadeExtra'), 'todosItensAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra']); pesquisarAtividadeExtra(true, 2);

                    if (!hasValue(dijit.byId('gridAtividadeExtra').itensSelecionados)) {
                        dijit.byId('gridAtividadeExtra').itensSelecionados = [];
                    } else {
                        dijit.byId('gridAtividadeExtra').itensSelecionados = dijit.byId("gridAtividadeExtra").store.objectStore.get(grid.itensSelecionados[0].cd_atividade_extra);
                    }

                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagem", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Email enviado com sucesso!");
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemRecorrencia', error);
        });
    });
}

function validaAulaExpermentalAtividadesSelecionadas(itensSelecionados) {
    if (hasValue(itensSelecionados)) {

        //every -> todos os itens do array tem que satisfazer a condição
         return itensSelecionados.every(function(item) {
             return ((item.cd_tipo_atividade_extra === EnumTipoAtividade.AULAEXPERIMENTAL));
        });

    }
}




function montarAtividadeExtraEnviarEmailRecorrenciasAcaoRelacionada(itensSelecionados) {
    try {
        var itens = [];
        $.each(itensSelecionados, function (idx, itemSelecionado) {
            var atividadeExtra = {
                cd_atividade_extra: itemSelecionado.cd_atividade_extra,
                cd_tipo_atividade_extra: itemSelecionado.cd_tipo_atividade_extra,
                dt_atividade_extra: itemSelecionado.dt_atividade_extra,
                hh_inicial: itemSelecionado.hh_inicial,
                hh_final: itemSelecionado.hh_final,
                nm_vagas: itemSelecionado.nm_vagas,
                cd_produto: itemSelecionado.cd_produto,
                cd_funcionario: itemSelecionado.cd_funcionario,
                ind_carga_horaria: itemSelecionado.ind_carga_horaria,
                ind_pagar_professor: itemSelecionado.ind_pagar_professor,
                tx_obs_atividade: itemSelecionado.tx_obs_atividade,
                cd_usuario_atendente: itemSelecionado.cd_usuario_atendente,
                cd_sala: itemSelecionado.cd_sala,
                cd_pessoa_escola: itemSelecionado.cd_pessoa_escola,
                id_calendario_academico: itemSelecionado.id_calendario_academico,
                hr_limite_academico: itemSelecionado.hr_limite_academico,
                id_email_enviado: itemSelecionado.id_email_enviado,
                cd_atividade_recorrrencia: itemSelecionado.cd_atividade_recorrencia,
                AtividadeRecorrencia: itemSelecionado.AtividadeRecorrencia,
                no_tipo_atividade_extra: itemSelecionado.no_tipo_atividade_extra
            }
            itens.push(atividadeExtra);
        });

        return itens;
    } catch (e) {
        postGerarLog(e);
    }
}

function EnviarEmailProspectsAcaoRelacionada(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegRecorrencia, null);
        else if (validaAulaExpermentalAtividadesSelecionadas(itensSelecionados) === false) {
            caixaDialogo(DIALOGO_ERRO, "Para Enviar Email é necessário que todos os itens selecionados sejam do tipo \"Aula Experimental\".", null);
        } //else if (!itensSelecionados.every(function (item) {
        //    return (item.cd_pessoa_escola + "") === dojo.byId("_ES0").value;
        //})) {
        //    caixaDialogo(DIALOGO_ERRO, "A funcionalidade de envio de email não é permitida para atividades de escolas diferentes da escola logada.", null);
        //}
        else {
            var itensSelecionadosSend = montarAtividadeExtraEnviarEmailRecorrenciasAcaoRelacionada(itensSelecionados);
        xhr.post({
            url: Endereco() + "/api/coordenacao/PostSendEmailProspectsAcaoRelacionada",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionadosSend)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                if (hasValue(data)) {
                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagem", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, data);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                } else {

                    var parametros = getParamterosURL();
                    var vlLanc = eval(parametros['lancamento']) ? 0 : 2;
                    var lanc = hasValue(dijit.byId('cbLancada_1').value, true) ? dijit.byId('cbLancada_1').value : vlancada;
                    buscarTodosItens(dijit.byId('gridAtividadeExtra'), 'todosItensAtividadeExtra', ['pesquisarAtividadeExtra', 'relatorioAtividadeExtra']); pesquisarAtividadeExtra(true, 2);

                    if (!hasValue(dijit.byId('gridAtividadeExtra').itensSelecionados)) {
                        dijit.byId('gridAtividadeExtra').itensSelecionados = [];
                    } else {
                        dijit.byId('gridAtividadeExtra').itensSelecionados = dijit.byId("gridAtividadeExtra").store.objectStore.get(grid.itensSelecionados[0].cd_atividade_extra);
                    }

                    var mensagensWeb = new Array();
                    apresentaMensagem("apresentadorMensagem", null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Email enviado com sucesso!");
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemRecorrencia', error);
        });
        };
    });
}

function pesquisarAlunosAtividades() {
    var cdAtividade = hasValue(dojo.byId('cd_atividade_extra').value) ? dojo.byId('cd_atividade_extra').value : 0;
    require([
          "dojo/_base/xhr",
          "dojo/data/ObjectStore",
          "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/GetAtividadeAluno?cdAtividadeExtra=" + cdAtividade,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_atividade_aluno"
        }).then(function (data) {
            try {
                showCarregando();
                var dataRetorno = jQuery.parseJSON(data).retorno;
                var alunos = [];
                if (hasValue(dataRetorno)) {
                    $.each(dataRetorno, function (idx, val) {

                        alunos.push({
                            no_pessoa: val.no_pessoa,
                            cd_atividade_aluno: val.cd_atividade_aluno,
                            cd_atividade_extra: val.cd_atividade_extra,
                            cd_aluno: val.cd_aluno,
                            ind_participacao: val.ind_participacao,
                            tx_obs_atividade_aluno: val.tx_obs_atividade_aluno,
                            dc_reduzido_pessoa_escola: val.dc_reduzido_pessoa_escola,
                            participou: val.participou,
                            isProspect: false
                        });

                    });
                }
                

                var grid = dijit.byId("gridAtividadesAlunos");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: alunos, idProperty: "cd_atividade_aluno" }) });
                grid.setStore(dataStore);
               
                quickSortObj(grid.itensSelecionados, 'cd_atividade_aluno');
                dijit.byId('tagAlunos').set('open', true);
                dijit.byId("gridAtividadesAlunos").update();
                showCarregando();

                pesquisarProspectsAtividades();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
        });
    });
}

function pesquisarProspectsAtividades() {
    var cdAtividade = hasValue(dojo.byId('cd_atividade_extra').value) ? dojo.byId('cd_atividade_extra').value : 0;
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/GetAtividadeProspect?cdAtividadeExtra=" + cdAtividade,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_atividade_prospect"
        }).then(function (data) {
                try {
                    
                    var grid = dijit.byId("gridAtividadesAlunos");

                    var dataRetorno = jQuery.parseJSON(data).retorno;
                    
                    var prospects = [];
                    if ((hasValue(grid) &&
                        hasValue(grid.store) &&
                        hasValue(grid.store.objectStore) &&
                        hasValue(grid.store.objectStore.data.length > 0))) {
                        prospects = grid.store.objectStore.data;
                    }
                    if (hasValue(dataRetorno)) {
                        $.each(dataRetorno,
                            function(idx, val) {

                                prospects.push({
                                    no_pessoa: val.no_pessoa,
                                    cd_atividade_aluno: val.cd_atividade_prospect,
                                    cd_atividade_extra: val.cd_atividade_extra,
                                    cd_aluno: val.cd_prospect,
                                    ind_participacao: val.ind_participacao,
                                    tx_obs_atividade_aluno: val.txt_obs_atividade_prospect,
                                    dc_reduzido_pessoa_escola: val.dc_reduzido_pessoa_escola,
                                    participou: val.participou,
                                    isProspect: true
                                });

                            });
                    }

                    var dataStore = new ObjectStore({ objectStore: new Memory({ data: prospects, idProperty: "cd_atividade_aluno" }) });
                    grid.setStore(dataStore);

                    quickSortObj(grid.itensSelecionados, 'cd_atividade_aluno');
                    dijit.byId('tagAlunos').set('open', true);
                    dijit.byId("gridAtividadesAlunos").update();
                    hideCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
            });
    });
}
//#endregion fim dos métodos de percistência

//#region Métodos auxiliares -  eventoEditarAtividadeExtra -  alterarAvaliacaoGrid - keepValuesAluno -  montarAlunoAtividadeExtra
function eventoEditarAtividadeExtra(itensSelecionados, ready, Memory, filteringSelect, ObjectStore) {
    try {
        var parametros = getParamterosURL();
        dijit.byId("cbProdutos")._onChangeActive = false;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            atividadeExtraPesq = new AtividadeExtraPesquisa(itensSelecionados[0]);
            
            require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {                
                xhr.post(Endereco() + "/coordenacao/getAtividadeExtraViewOnDbClik", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson(atividadeExtraPesq)
                }).then(function (dataAtividadeExtra) {
                    try {
                        showCarregando();
                        if (!hasValue(dataAtividadeExtra.erro)) {
                            limparFormAtividadeExtra(ready, Memory, filteringSelect, ObjectStore);
                            loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, "cbTiposAtividades");

                            obterRecursosAtividadeExtra(atividadeExtraPesq, itensSelecionados);
                            

                            loadMultiCurso(dataAtividadeExtra.retorno.cursos, "cbCursos");
                            if (!eval(parametros['lancamento']))
                                if ((dataAtividadeExtra.retorno.cursos != null) &&
                                    (dataAtividadeExtra.retorno.cursos.length > 0))
                                    dijit.byId("cbCursos").set('disabled', false);

                            loadSala(dataAtividadeExtra.retorno.salasDisponiveis, "cbSalas");
                             

                            pesquisarEscolasVinculadasUsuarioAtividade();
                            dojo.byId('abriuEscola').value = false;
                            dojo.byId('nm_alunos').value = dataAtividadeExtra.retorno.nm_total_alunos;

                            //dojo.byId('cdUsuario').value = dataAtividadeExtra.retorno.cd_usuario_atendente;

                            //getLimpar('#formAtividadeExtra');
                            //apresentaMensagem('apresentadorMensagem', '');
                            //dijit.byId('gridAtividadeExtra').itemSelecionado = itensSelecionados[0];
                            //keepValues(null, dijit.byId('gridAtividadeExtra'), true);
                            //checkedValuesTrue(dataAtividadeExtra.retorno.cursos, dijit.byId('gridAtividadeExtra').itemSelecionado.cd_cursos, "cbCursos");
                            //dijit.byId("cadAtividadeExtra").show();
                            //IncluirAlterar(0, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagemAtividadeExtra', 'divCancelarAtividadeExtra', 'divLimparAtividadeExtra');

                        } else
                            apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
                        
                    }
                    catch (e) {           
                        hideCarregando();
                        postGerarLog(e);
                    }                    
                },
                function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
                });
            });
        }
    }
    catch (er) {
        hideCarregando();
        postGerarLog(er);
    }
}


function eventoGerarRecorrenciaAtividadeExtra(itensSelecionados, ready, Memory, filteringSelect, ObjectStore) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegRecorrencia, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegRecorrencia, null);
        /*else if (itensSelecionados[0].cd_tipo_atividade_extra !== EnumTipoAtividade.AULAEXPERIMENTAL) {
            caixaDialogo(DIALOGO_ERRO, "A funcionalidade de recorrência só é permitida para atividades do tipo \"Aula Experimental\".", null);
        }*/ else if ((itensSelecionados[0].cd_pessoa_escola + "") !== dojo.byId("_ES0").value) {
            caixaDialogo(DIALOGO_ERRO, "A funcionalidade de recorrência não é permitida para atividades de escolas diferentes da escola logada.", null);
        }
        else {
            var mensagensWeb = new Array();
            apresentaMensagem("apresentadorMensagemRecorrencia", null);
            //preeche os combos
            if (!hasValue(dijit.byId("id_tipo_recorrencia"))) {
                criarOuCarregarCompFiltering("id_tipo_recorrencia",
                    [{ name: "Diariamente", id: "1" },
                        { name: "Semanalmente", id: "2" },
                        { name: "Quinzenalmente", id: "3" },
                        { name: "Mensalmente", id: "4" },
                        { name: "Anualmente", id: "5" }
                    ], "",
                    null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            }
            
            if (!hasValue(dijit.byId("nm_frequencia"))) {
                criarOuCarregarCompFiltering("nm_frequencia",
                    [{ name: "1", id: "1" },
                        { name: "7", id: "2" },
                        { name: "15", id: "3" },
                        { name: "30", id: "4" },
                        { name: "365", id: "5" }

                    ], "",
                    null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            }


            if (!hasValue(dijit.byId("nm_eventos"))) {
                criarOuCarregarCompFiltering("nm_eventos",
                    [{ name: "1", id: "1" },
                        { name: "2", id: "2" },
                        { name: "3", id: "3" },
                        { name: "4", id: "4" },
                        { name: "5", id: "5" },
                        { name: "6", id: "6" },
                        { name: "7", id: "7" }
                    ], "",
                    null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            }
            //fim preeche os combos

            if (itensSelecionados[0].cd_atividade_recorrencia === null &&
                itensSelecionados[0].AtividadeRecorrencia !== null &&
                itensSelecionados[0].AtividadeRecorrencia.cd_atividade_extra ===
                itensSelecionados[0].cd_atividade_extra) {
                //habilita excluir//desabilita gerar

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("DeleteRecorrencia").domNode, 'display', '');
                });

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("gerarRecorrencia").domNode, 'display', 'none');
                });

                if (itensSelecionados[0].id_email_enviado === false && itensSelecionados[0].cd_tipo_atividade_extra === EnumTipoAtividade.AULAEXPERIMENTAL) {
                    require(['dojo/dom-style', 'dijit/registry'],
                        function(domStyle, registry) {
                            domStyle.set(registry.byId("enviarEmailAllRecorrencias").domNode, 'display', '');
                        });
                } else {
                    require(['dojo/dom-style', 'dijit/registry'],
                        function (domStyle, registry) {
                            domStyle.set(registry.byId("enviarEmailAllRecorrencias").domNode, 'display', 'none');
                        });
                }
               

                dijit.byId("id_tipo_recorrencia").reset();
                dijit.byId("id_tipo_recorrencia").set("value", itensSelecionados[0].AtividadeRecorrencia.id_tipo_recorrencia);

                dijit.byId("nm_frequencia").reset();
                dijit.byId("nm_frequencia").set("value", itensSelecionados[0].AtividadeRecorrencia.nm_frequencia);

                if (itensSelecionados[0].AtividadeRecorrencia.nm_eventos > 0 &&
                    itensSelecionados[0].AtividadeRecorrencia.dt_limite === null) {
                    dijit.byId("id_data_termino_2").set("checked", true);
                    dijit.byId("nm_eventos").reset();
                    dijit.byId("nm_eventos").set("value", itensSelecionados[0].AtividadeRecorrencia.nm_eventos);
                } else if (itensSelecionados[0].AtividadeRecorrencia.nm_eventos === 0 &&
                    itensSelecionados[0].AtividadeRecorrencia.dt_limite !== null) {
                    dijit.byId("id_data_termino_1").set("checked", true);
                    dijit.byId("dt_limite").reset();
                    dijit.byId("dt_limite").set("value", itensSelecionados[0].AtividadeRecorrencia.dt_limite);
                }
               

                if (dijit.byId("id_data_termino_1").checked == true) {
                    dijit.byId("dt_limite").set("required", true);
                } else {
                    dijit.byId("dt_limite").set("required", false);
                }



                if (dijit.byId("id_data_termino_2").checked == true) {
                    dijit.byId("nm_eventos").set("required", true);
                } else {
                    dijit.byId("nm_eventos").set("required", false);
                }

                getStatusRecorrencia();


            } else if (itensSelecionados[0].cd_atividade_recorrencia !== null &&
                itensSelecionados[0].AtividadeRecorrencia !== null &&
                itensSelecionados[0].AtividadeRecorrencia.cd_atividade_recorrencia ===
                itensSelecionados[0].cd_atividade_recorrencia) {
                //so visualiza

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("DeleteRecorrencia").domNode, 'display', 'none');
                });

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("gerarRecorrencia").domNode, 'display', 'none');
                });

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("enviarEmailAllRecorrencias").domNode, 'display', 'none');
                });

                dijit.byId("id_tipo_recorrencia").reset();
                dijit.byId("id_tipo_recorrencia").set("value", itensSelecionados[0].AtividadeRecorrencia.id_tipo_recorrencia);

                dijit.byId("nm_frequencia").reset();
                dijit.byId("nm_frequencia").set("value", itensSelecionados[0].AtividadeRecorrencia.nm_frequencia);

                if (itensSelecionados[0].AtividadeRecorrencia.nm_eventos > 0 &&
                    itensSelecionados[0].AtividadeRecorrencia.dt_limite === null) {
                    dijit.byId("id_data_termino_2").set("checked", true);
                    dijit.byId("nm_eventos").reset();
                    dijit.byId("nm_eventos").set("value", itensSelecionados[0].AtividadeRecorrencia.nm_eventos);
                } else if (itensSelecionados[0].AtividadeRecorrencia.nm_eventos === 0 &&
                    itensSelecionados[0].AtividadeRecorrencia.dt_limite !== null) {
                    dijit.byId("id_data_termino_1").set("checked", true);
                    dijit.byId("dt_limite").reset();
                    dijit.byId("dt_limite").set("value", itensSelecionados[0].AtividadeRecorrencia.dt_limite);
                }


                if (dijit.byId("id_data_termino_1").checked == true) {
                    dijit.byId("dt_limite").set("required", true);
                } else {
                    dijit.byId("dt_limite").set("required", false);
                }



                if (dijit.byId("id_data_termino_2").checked == true) {
                    dijit.byId("nm_eventos").set("required", true);
                } else {
                    dijit.byId("nm_eventos").set("required", false);
                }

                getStatusRecorrencia();

            }else if (itensSelecionados[0].cd_atividade_recorrencia === null &&
                itensSelecionados[0].AtividadeRecorrencia === null) {
                //habilita gerar

                

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("DeleteRecorrencia").domNode, 'display', 'none');
                });

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("gerarRecorrencia").domNode, 'display', '');
                });

                require(['dojo/dom-style', 'dijit/registry'], function (domStyle, registry) {
                    domStyle.set(registry.byId("enviarEmailAllRecorrencias").domNode, 'display', 'none');
                });

                dijit.byId("id_tipo_recorrencia").reset();
                dijit.byId("nm_frequencia").reset();
                dijit.byId("nm_eventos").reset();
                dijit.byId("dt_limite").reset();
                dijit.byId("id_data_termino_1").set("checked", true);

               
            }

                //var mensagensWeb = new Array();
                //mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Mensagem teste.');
                //apresentaMensagem('apresentadorMensagemRecorrencia', mensagensWeb);

                
                dijit.byId('dialogGerarRecorrencia').show();
           
        }
    }
    catch (er) {
        postGerarLog(er);
    }
}

function obterRecursosAtividadeExtraDbClick(atividadeExtraPesq, cursos, gridAtividadeExtra, escolaAtividadeSelecionada) {
    //showCarregando();
    var parametros = getParamterosURL();
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/obterRecursosAtividadeExtra", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(atividadeExtraPesq)
        }).then(function (dataAtividadeExtra) {
            try {
                showCarregando();
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadProfessor(dataAtividadeExtra.retorno.professores, "cbResponsaveis");
                    loadProduto(dataAtividadeExtra.retorno.produtos, "cbProdutos");

                    if (hasValue(dataAtividadeExtra.retorno.produtos)) {
                        console.log(dataAtividadeExtra.retorno.produtos);
                        if (!eval(parametros['lancamento']))
                            dijit.byId("cbCursos").set('disabled', false);
                    }

                    keepValues(null, gridAtividadeExtra, false);
                    dojo.byId('cdUsuario').value = dataAtividadeExtra.retorno.cd_usuario_atendente;
                    dijit.byId("cadAtividadeExtra").show();
                    dijit.byId("tagAlunos").set("open", false);
                    checkedValuesTrue(cursos, gridAtividadeExtra.itemSelecionado.cd_cursos, "cbCursos");

                    if ((escolaAtividadeSelecionada + "") !== dojo.byId("_ES0").value) {
                        if (!eval(parametros['lancamento'])) {
                            toogleCamposAtividadeExtraEscolaLogada(true);
                        }
                    } else {
                        if (!eval(parametros['lancamento'])) {
                            toogleCamposAtividadeExtraEscolaLogada(false);
                        }
                    }
                    
                } else {
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
                }
            } catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
            //showCarregando();
        })
    })
}

function toogleCamposAtividadeExtraEscolaLogada(habilita) {
    try {
        dijit.byId('cbTiposAtividades').set('disabled', habilita);
        dijit.byId('nroVagas').set('disabled', habilita);
        dijit.byId('hrInicial').set('disabled', habilita);
        dijit.byId('hrFinal').set('disabled', habilita);
        dijit.byId('dataAtividade').set('disabled', habilita);
        dijit.byId('cbProdutos').set('disabled', habilita);
        dijit.byId('cbResponsaveis').set('disabled', habilita);
        dijit.byId('ckCarga').set('disabled', habilita);
        dijit.byId('ckPagar').set('disabled', habilita);
        dijit.byId('ckPortal').set('disabled', habilita);
        dijit.byId('hrLimite').set('disabled', habilita);
        dijit.byId('cbSalas').set('disabled', habilita);
        dijit.byId('txObs').set('disabled', habilita);
        dijit.byId('cbCursos').set('disabled', habilita);
    } catch (e) {
        postGerarLog(e);
    } 
}

function toogleCamposAtividadeExtraEscolaLogadaAcaoRelEdit(habilita) {
    try {
        dijit.byId('cbTiposAtividades').set('disabled', habilita);
        dijit.byId('nroVagas').set('disabled', habilita);
        dijit.byId('hrInicial').set('disabled', habilita);
        dijit.byId('hrFinal').set('disabled', habilita);
        dijit.byId('dataAtividade').set('disabled', habilita);
        dijit.byId('cbProdutos').set('disabled', habilita);
        dijit.byId('cbResponsaveis').set('disabled', habilita);
        dijit.byId('ckCarga').set('disabled', habilita);
        dijit.byId('ckPagar').set('disabled', habilita);
        dijit.byId('ckPortal').set('disabled', habilita);
        dijit.byId('hrLimite').set('disabled', habilita);
        dijit.byId('txObs').set('disabled', habilita);
        dijit.byId('cbCursos').set('disabled', habilita);
        dijit.byId('cbSalas').set('disabled', habilita);
        dijit.byId('txAtendente').set('disabled', habilita);
    } catch (e) {
        postGerarLog(e);
    }
}

function obterRecursosAtividadeExtraClickCancel(atividadeExtraPesq, cursos, gridAtividadeExtraCancel) {
    //showCarregando();
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/obterRecursosAtividadeExtra", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(atividadeExtraPesq)
        }).then(function (dataAtividadeExtra) {
            try {
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadProfessor(dataAtividadeExtra.retorno.professores, "cbResponsaveis");
                    loadProduto(dataAtividadeExtra.retorno.produtos, "cbProdutos");

                    keepValues(null, gridAtividadeExtraCancel, false);
                    checkedValuesTrue(cursos, gridAtividadeExtraCancel.itemSelecionado.cd_cursos, "cbCursos");

                } else {
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
                }
                hideCarregando();

            } catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        })
    })
}

function obterRecursosAtividadeExtra(atividadeExtraPesq, itensSelecionados) {
    //showCarregando();

    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/obterRecursosAtividadeExtra", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(atividadeExtraPesq)
        }).then(function (dataAtividadeExtra) {
            try {
                showCarregando();
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadProfessor(dataAtividadeExtra.retorno.professores, "cbResponsaveis");
                    loadProduto(dataAtividadeExtra.retorno.produtos, "cbProdutos");

                    dojo.byId('cdUsuario').value = dataAtividadeExtra.retorno.cd_usuario_atendente;

                    getLimpar('#formAtividadeExtra');
                    apresentaMensagem('apresentadorMensagem', '');
                    dijit.byId('gridAtividadeExtra').itemSelecionado = itensSelecionados[0];
                    keepValues(null, dijit.byId('gridAtividadeExtra'), true);
                    checkedValuesTrue(dataAtividadeExtra.retorno.cursos, dijit.byId('gridAtividadeExtra').itemSelecionado.cd_cursos, "cbCursos");

                    var parametros = getParamterosURL();
                    if ((itensSelecionados[0].cd_pessoa_escola + "") !== dojo.byId("_ES0").value) {
                        if (!eval(parametros['lancamento'])) {
                            toogleCamposAtividadeExtraEscolaLogadaAcaoRelEdit(true);
                        }
                    } else {
                        if (!eval(parametros['lancamento'])) {
                            toogleCamposAtividadeExtraEscolaLogadaAcaoRelEdit(false);
                        }
                    }


                    dijit.byId("cadAtividadeExtra").show();
                    IncluirAlterar(0, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagemAtividadeExtra', 'divCancelarAtividadeExtra', 'divLimparAtividadeExtra');
                } else {
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
                }
            } catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        })
    })
}
//Edita um registro na grid de alunos atividade
function alterarAvaliacaoGrid(grid) {
    try{
        apresentaMensagem('apresentadorMensagemAluno', null);
        var cdAluno = dojo.byId("cd_aluno").value == "" ? 0 : dojo.byId("cd_aluno").value;
        for (var l = 0; l < grid._by_idx.length ; l++) {
            // Altera o registro na grade:
            if (grid._by_idx[l].item.cd_aluno == cdAluno) {
                grid._by_idx[l].item.ind_participacao = dojo.byId("ckParticipouAluno").checked;
                grid._by_idx[l].item.tx_obs_atividade_aluno = dojo.byId("descObs").value;
            }
        }
        grid.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//restaura dados da grade de aluno atividade
function keepValuesAluno(grid) {
    try{
        dojo.byId("cd_aluno").value = grid._by_idx[0].item.cd_aluno;
        dojo.byId("ckParticipouAluno").value = grid._by_idx[0].item.id_avaliacao_ativa == true ? dijit.byId("ckParticipouAluno").set("value", true) : dijit.byId("ckParticipouAluno").set("value", false);
        dojo.byId("descObs").value = grid._by_idx[0].item.tx_obs_atividade_aluno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAlunoAtividadeExtra() {
    try{
        var alunos = [];
        var gridAlunoAtividade = dijit.byId("gridAtividadesAlunos");

        hasValue(gridAlunoAtividade) ? gridAlunoAtividade.store.save() : null;
        if (hasValue(gridAlunoAtividade) && hasValue(gridAlunoAtividade._by_idx))
            var data = gridAlunoAtividade.store.objectStore.data;
        else alunos = null;
        if (hasValue(gridAlunoAtividade) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                if (val.isProspect === false) {
                    alunos.push({
                        cd_atividade_aluno: val.cd_atividade_aluno,
                        cd_atividade_extra: val.cd_atividade_extra,
                        cd_aluno: val.cd_aluno,
                        ind_participacao: val.ind_participacao,
                        tx_obs_atividade_aluno: val.tx_obs_atividade_aluno
                    });
                }
                
            });
        return alunos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAtividadeExtraForRecorrencia(itemSelecionado) {
    try {
        var atividadeExtra = {
            cd_atividade_extra: itemSelecionado.cd_atividade_extra,
            cd_tipo_atividade_extra: itemSelecionado.cd_tipo_atividade_extra,
            dt_atividade_extra: itemSelecionado.dt_atividade_extra,
            hh_inicial: itemSelecionado.hh_inicial,
            hh_final: itemSelecionado.hh_final,
            nm_vagas: itemSelecionado.nm_vagas,
            cd_produto: itemSelecionado.cd_produto,
            cd_funcionario: itemSelecionado.cd_funcionario,
            ind_carga_horaria: itemSelecionado.ind_carga_horaria,
            ind_pagar_professor: itemSelecionado.ind_pagar_professor,
            tx_obs_atividade: itemSelecionado.tx_obs_atividade,
            cd_usuario_atendente: itemSelecionado.cd_usuario_atendente,
            cd_sala: itemSelecionado.cd_sala,
            cd_pessoa_escola: itemSelecionado.cd_pessoa_escola,
            id_calendario_academico: itemSelecionado.id_calendario_academico,
            hr_limite_academico: itemSelecionado.hr_limite_academico,
            id_email_enviado: itemSelecionado.id_email_enviado,
            cd_atividade_recorrrencia: itemSelecionado.cd_atividade_recorrrencia,
            AtividadeRecorrencia: montarRecorrenciaAtividadeExtra(itemSelecionado.cd_atividade_extra)
        }

        return atividadeExtra;
    } catch (e) {
        postGerarLog(e);
    } 
}


function montarRecorrenciaAtividadeExtra(cd_atividade_extra) {
    try {
        var dataLimite = hasValue(dojo.byId("dt_limite").value)
            ? dojo.date.locale.parse(dojo.byId("dt_limite").value,
                { formatLength: 'short', selector: 'date', locale: 'pt-br' })
            : null;
        var recorrencia = {
            cd_atividade_recorrencia: 0,
            cd_atividade_extra: cd_atividade_extra,
            id_tipo_recorrencia: dijit.byId("id_tipo_recorrencia").value,
            nm_frequencia: (dijit.byId("id_data_termino_2").checked === true) ? dijit.byId("nm_frequencia").value : null,
            dt_limite: (dijit.byId("id_data_termino_1").checked === true) ? dataLimite : null,
            nm_eventos: dijit.byId("nm_eventos").value
        }

        return recorrencia;
    } catch (e) {

    }

}

function montarProspectAtividadeExtra() {
    try {
        var prospects = [];
        var gridAlunoAtividade = dijit.byId("gridAtividadesAlunos");

        hasValue(gridAlunoAtividade) ? gridAlunoAtividade.store.save() : null;
        if (hasValue(gridAlunoAtividade) && hasValue(gridAlunoAtividade._by_idx))
            var data = gridAlunoAtividade._by_idx;
        else prospects = null;
        if (hasValue(gridAlunoAtividade) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                if (val.item.isProspect === true) {
                    prospects.push({
                        cd_atividade_prospect: hasValue(val.item.cd_atividade_aluno)? val.item.cd_atividade_aluno: 0,
                        cd_atividade_extra: hasValue(val.item.cd_atividade_extra) ? val.item.cd_atividade_extra : 0,
                        cd_prospect: val.item.cd_aluno,
                        ind_participacao: val.item.ind_participacao,
                        txt_obs_atividade_prospect: val.item.tx_obs_atividade_aluno
                    });
                }

            });
        return prospects;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region Método para retornar registros na grade de Aluno - consistirNumeroVagas - ParticipouNao - pesquisarAlunoAtividadeExtra
function retornarAlunoFK() {
    var nroVagas = hasValue(dijit.byId('nroVagas')) ? dijit.byId('nroVagas') : 0;
    if (nroVagas == 0){
        caixaDialogo(DIALOGO_ERRO, msgNroVagasZerada, null);
        return false;
    }
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try{
                var gridAtividadesAlunos = dijit.byId("gridAtividadesAlunos");
                var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridAtividadesAlunos = (hasValue(gridAtividadesAlunos) && hasValue(gridAtividadesAlunos.store.objectStore.data)) ? gridAtividadesAlunos.store.objectStore.data : [];

                    if (storeGridAtividadesAlunos != null && storeGridAtividadesAlunos.length > 0) {
                        if (consistirNumeroVagas(gridPesquisaAluno.itensSelecionados) == false)
                            return false;
                        $.each(gridPesquisaAluno.itensSelecionados, function (idx, value) {
                            insertObjSort(gridAtividadesAlunos.store.objectStore.data, "cd_aluno", { cd_aluno: value.cd_aluno, no_pessoa: value.no_pessoa, ind_participacao: false, tx_obs_atividade_aluno: "", dc_reduzido_pessoa_escola: value.dc_reduzido_pessoa_escola, isProspect: false });
                        });
                        gridAtividadesAlunos.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAtividadesAlunos.store.objectStore.data }) }));

                    } else {
                        var dados = [];
                        if (consistirNumeroVagas(gridPesquisaAluno.itensSelecionados) == false)
                            return false;
                        $.each(gridPesquisaAluno.itensSelecionados, function (index, val) {
                            insertObjSort(dados, "cd_aluno", { cd_aluno: val.cd_aluno, no_pessoa: val.no_pessoa, ind_participacao: false, tx_obs_atividade_aluno: "", dc_reduzido_pessoa_escola: val.dc_reduzido_pessoa_escola, isProspect: false });
                        });
                        gridAtividadesAlunos.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));

                    }
                    dijit.byId("proAluno").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function retornarAlunoPesquisaFK() {
    try{
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                return false;
        } else {
            dojo.byId('cdAlunoPesq').value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId('alunoPesq').value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
        }
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function abrirFKProspectOrigemAluno() {
    try {
        limparPesquisaProspectFK();
        dojo.byId("idOrigemProspectFK").value = ORIGCADALUNO;
        pesquisarProspectFK();

        //Remove o tipo da FK:
        dojo.byId("trTipoPesquisaFKProspect").style.display = "none";

        dijit.byId("cadProspectFollowUpFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarProspectFKOrigemAluno() { //selecionaProspectFK
    try {
        var nroVagas = hasValue(dijit.byId('nroVagas')) ? dijit.byId('nroVagas') : 0;
        if (nroVagas == 0) {
            caixaDialogo(DIALOGO_ERRO, msgNroVagasZerada, null);
            return false;
        }

        require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridAtividadesAlunos = dijit.byId("gridAtividadesAlunos");
                var gridPesquisaProspect = dijit.byId("gridPesquisaProspect");

                if (!hasValue(gridPesquisaProspect.itensSelecionados) || gridPesquisaProspect.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridAtividadesAlunos = (hasValue(gridAtividadesAlunos) && hasValue(gridAtividadesAlunos.store.objectStore.data)) ? gridAtividadesAlunos.store.objectStore.data : [];

                    if (storeGridAtividadesAlunos != null && storeGridAtividadesAlunos.length > 0) {
                        if (consistirNumeroVagas(gridPesquisaProspect.itensSelecionados) == false)
                            return false;
                        $.each(gridPesquisaProspect.itensSelecionados, function (idx, value) {
                            insertObjSort(gridAtividadesAlunos.store.objectStore.data, "cd_aluno", { cd_aluno: value.cd_prospect, ind_participacao: false, no_pessoa: value.no_pessoa, tx_obs_atividade_aluno: "", dc_reduzido_pessoa_escola: value.dc_reduzido_pessoa_escola, isProspect: true });
                        });
                        gridAtividadesAlunos.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAtividadesAlunos.store.objectStore.data }) }));

                    } else {
                        var dados = [];
                        if (consistirNumeroVagas(gridPesquisaProspect.itensSelecionados) == false)
                            return false;
                        $.each(gridPesquisaProspect.itensSelecionados, function (index, val) {
                            insertObjSort(dados, "cd_aluno", { cd_aluno: val.cd_prospect, no_pessoa: val.no_pessoa, ind_participacao: false, tx_obs_atividade_aluno: "", dc_reduzido_pessoa_escola: val.dc_reduzido_pessoa_escola, isProspect: true  });
                        });
                        gridAtividadesAlunos.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));

                    }
                    dijit.byId("cadProspectFollowUpFK").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        /*var gridPesquisaProspect = dijit.byId("gridPesquisaProspect");

        if (!hasValue(gridPesquisaProspect.itensSelecionados) || gridPesquisaProspect.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
        if (gridPesquisaProspect.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
            valido = false;
        }
        else {
            if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect)) {
                dojo.byId('cd_prospect').value = gridPesquisaProspect.itensSelecionados[0].cd_prospect;
                //Chama função para carregar os dados do prospect para um novo aluno.
                configuraDadosProspect(gridPesquisaProspect.itensSelecionados[0].cd_prospect, dojo.data.ObjectStore, dojo.store.Memory, function () {
                    dijit.byId("limparProspect").set("disabled", false);
                });
            }
        }
        if (!valido)
            return false;
        dijit.byId("cadProspectFollowUpFK").hide();*/
    }
    catch (e) {
        postGerarLog(e);
    }
}


function consistirNumeroVagas(itens) {
    try{
        var gridAluno = hasValue(dijit.byId('gridAtividadesAlunos')) ? dijit.byId('gridAtividadesAlunos') : 0;
        var nroVagas = hasValue(dijit.byId('nroVagas')) ? dijit.byId('nroVagas').get('value') : 0;
        if (nroVagas > 0) {
        var nroAlunos = 0;
        if (hasValue(itens) && itens.length > 0 && hasValue(gridAluno.store.objectStore.data))
            nroAlunos = itens.length + gridAluno.store.objectStore.data.length;
            if (nroAlunos > nroVagas) {
                caixaDialogo(DIALOGO_AVISO, msgNroVagasMenorAlunos, null);
                return false;
            } else return true;

        } else {
            caixaDialogo(DIALOGO_ERRO, msgNroVagasZerada, null);
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ParticipouNao(participou) {
    try{
        var itens = dijit.byId("gridAtividadesAlunos").itensSelecionados;
        if (!hasValue(itens) || itens.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else {
            var grid = dijit.byId("gridAtividadesAlunos").store.objectStore.data;
            var parametros = getParamterosURL();
            for (var i = 0; i < grid.length; i++) {
                for (var j = 0; j < itens.length; j++) {
                    if (grid[i].cd_aluno == itens[j].cd_aluno)
                        if (participou) {
                            itens[j].ind_participacao = true;
                            grid[i].ind_participacao = true;
                        }
                        else {
                            itens[j].ind_participacao = false;
                            grid[i].ind_participacao = false;
                        }
                }
                if (eval(parametros['lancamento']))
                    grid[i].tx_obs_atividade_aluno += "  ";
            }
            dijit.byId("gridAtividadesAlunos").store.save();
            dijit.byId("gridAtividadesAlunos").update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

function retornarAlunoFKAtivdadeExtra() {
    if (!hasValue(dojo.byId("cadAtividadeExtra").style.display))
        retornarAlunoFK();
    else
        retornarAlunoPesquisaFK();
}

function abrirAlunoFk() {
    
    //if (hasValue(dijit.byId("cbCursos").value)) {

        dojo.byId('tipoRetornoAlunoFK').value = ATIVIDADEEXTRA;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        hasValue(dijit.byId("cbCursos").value) ? pesquisarAlunoFKAtividadeExtra(true, dijit.byId("cbCursos").value) : pesquisarAlunoAtividadeFK(true);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();

    //} else {
    //    caixaDialogo(DIALOGO_ERRO, msgNenhumCursoSelecionado, null);
    //    return false;

    //}
    
}


//variavel utilizada para diferenciar se o check de todos foi desmarcado por click ou por ter itens selecionados 
var cbCursos = false;

function getValues() {
    try {
        selected = dijit.byId("cbCursos").get('value');

        //se o item todos foi selecionado
        if (selected.indexOf(selected, "-1") == -1 && (!cbCursos || (hasValue(cbCursos) && cbCursos.value == "-1")) && dijit.byId("cbCursos").options[0].selected == true) {
            console.log('clicou');
            for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
                dijit.byId("cbCursos").options[i].selected = true;
            }
            cbCursos = true; //marcou o check de todos          
        }

        //conta a quantidade de itens selecionados
        var contSelected = 0;
        for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
            if (dijit.byId("cbCursos").options[i].selected == true) {
                contSelected++;
            }
        }

        //se já clicou em todos mas nao esta selecionado(desmarca todos)
        if (cbCursos && dijit.byId("cbCursos").options[0].selected == false) {

            for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
                dijit.byId("cbCursos").options[i].selected = false;
            }
            cbCursos = false;//desmarcou o check de todos(click) 

        }

        //se ja clicou em todos mas desmarcou algum (desmarca o check todos)
        if (dijit.byId("cbCursos").options.length != contSelected && cbCursos == true) {
            dijit.byId("cbCursos").options[0].selected = false;
            cbCursos = false;//desmarcou o check de todos(tem itens selecionados ) 
        }

    }
    catch (e) {
        postGerarLog(e);
    }

}

/*
function findAllEmpresasUsuarioComboFiltroEscolaTelaAtividadeExtra() {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/empresa/findAllEmpresasUsuarioComboFK",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_pessoa"
        }).then(function (data) {
                try {
                    showCarregando();

                    var dataRetorno = jQuery.parseJSON(data);
                    if (hasValue(dataRetorno)) {
                        var dataCombo = dataRetorno.map(function (item, index, array) {
                            return { name: item.dc_reduzido_pessoa, id: item.cd_pessoa + "" };
                        });
                        loadSelect(dataRetorno, "escolaFiltroTelaAtividadeExtra", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
                    }
                    showCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
    });
}*/
