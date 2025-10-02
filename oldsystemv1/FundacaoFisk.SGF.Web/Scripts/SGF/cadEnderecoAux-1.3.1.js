var ENDERECOPRINCIPAL = 1, OUTROSENDERECOS = 2, ENDERECOPESSOARELAC = 3, ENDERECOPRINCIPALRESPONSAVELMATRICULA = 4;
var LAYOUTPESQUISALOGRADOURO = 1, LAYOUTPESQUISACEP = 2, LAYOUTPADRAO = 3, LAYOUTCEPCOMPLETO = 4;
var PAPELCPF = 1, PAPELCNPJ = 2, PAPELCPFCNPJ = 0;
var CEPLOGRADOURO = 1, CEPUNICO = 2;

function carregarCidadePorEstado(cd_estado, idCidade, idApresentador) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/GetCidade?idEstado=" + cd_estado,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
                if (hasValue(data) || data.length == 0)
                    criarOuCarregarCompFiltering(idCidade, data, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(idApresentador, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarBairroPorCidade(cd_cidade, idBairro, idApresentador) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getBairroPorCidade?cd_cidade=" + cd_cidade + "&cd_bairro=0",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                data = data.retorno;
                if (hasValue(data) || data.length == 0)
                    criarOuCarregarCompFiltering(idBairro, data, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
            }
            catch (e) {
                postGerarLog(e);
            }

        },
        function (error) {
            apresentaMensagem(idApresentador, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEnderecoOrCadastrar(data, tipoPesquisaCep, idMsg, sugerir_fk) {
    dojo.xhr.post({
        url: Endereco() + "/api/localidade/postEnderecoBuscaCep",
        handleAs: "json",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify(data)
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data).retorno;
            if (data != null && hasValue(data.num_cep) && data.num_cep.substring(data.num_cep.length - 3, data.num_cep.length) == "000" && sugerir_fk)
                caixaDialogo(DIALOGO_CONFIRMAR, msgCEPUnico,
                    function executaRetorno() {
                        if (!hasValue(dijit.byId("gridLogradouroFK")))
                            montargridPesquisaLogradouroFK(function () { abrirFKLogradouroAtravesBuscaCEP(data, tipoPesquisaCep); });
                        else
                            abrirFKLogradouroAtravesBuscaCEP(data, tipoPesquisaCep, idMsg);
                    }, function executaRetorno() { setarEnderecoPorCep(data, tipoPesquisaCep, idMsg) });
            else
                setarEnderecoPorCep(data, tipoPesquisaCep, idMsg);
        }
        catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem(idMsg, error);
    });
}

function setarEnderecoPorCep(data, tipoPesquisaCep, idMsg) {
    if (hasValue(tipoPesquisaCep))
        switch (tipoPesquisaCep) {
            case ENDERECOPRINCIPAL:
                setarRetornoEnderecoPrincipal(data, false, dijit.byId("cep").value, idMsg);
                break;
            case OUTROSENDERECOS:
                setarRetornoEnderecoOutrosEnderecos(data, false, dijit.byId("cepOutrosEnd").value, idMsg);
                break;
            case ENDERECOPRINCIPALRESPONSAVELMATRICULA:
                setarRetornoEnderecoPrincipalMat(data, false, dijit.byId("cepMat").value, "apresMsgNewRelacMat");
                break;
        }
}

function abrirFKLogradouroAtravesBuscaCEP(data, tipoPesquisaCep) {
    if (hasValue(tipoPesquisaCep))
        switch (tipoPesquisaCep) {
            case ENDERECOPRINCIPAL:
                abrirPesquisaLogradouroFK(data);
                var compCep = dijit.byId("cep");
                compCep._onChangeActive = false;
                compCep.set("value", "");
                compCep._onChangeActive = true;
                break;
            case OUTROSENDERECOS:
                abrirPesquisaLogradouroFKOutrosEnd(data);
                var compCep = dijit.byId("cepOutrosEnd");
                compCep._onChangeActive = false;
                compCep.set("value", "");
                compCep._onChangeActive = true;
                break;
            case ENDERECOPRINCIPALRESPONSAVELMATRICULA:
                abrirPesquisaLogradouroFKMat(data);
                var compCep = dijit.byId("cepMat");
                compCep._onChangeActive = false;
                compCep.set("value", "");
                compCep._onChangeActive = true;
                break;
        }
}

function pesquisarCepPessoa(nm_cep, tipoPesquisaCep, idApresentador) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getEnderecoCEP?nm_cep=" + nm_cep,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            data = jQuery.parseJSON(data).retorno;
            if (hasValue(tipoPesquisaCep))
                switch (tipoPesquisaCep) {
                    case ENDERECOPRINCIPAL:
                        dojo.byId("setDefaultValueCEP").value = ENDERECOPRINCIPAL;
                        setarRetornoEnderecoPrincipal(data, true, nm_cep, 'apresentadorMensagemPessoa');
                        break;
                    case OUTROSENDERECOS:
                        dojo.byId("setDefaultValueCEP").value = OUTROSENDERECOS;
                        setarRetornoEnderecoOutrosEnderecos(data, true, nm_cep, 'apresentadorMsgOutosEnderecos');
                        break;
                    case ENDERECOPRINCIPALRESPONSAVELMATRICULA:
                        //dojo.byId("setDefaultValueCEP").value = ENDERECOPRINCIPALRESPONSAVELMATRICULA;
                        setarRetornoEnderecoPrincipalMat(data, true, nm_cep, "apresMsgNewRelacMat");
                        break;
                }
        },
        function (error) {
            apresentaMensagem(idApresentador, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configurarLayoutEndereco(layout, idEstado, idCidade, idBairro, idRua, idCep, btnCadCidade, btnCadBairro, btnCadLog) {
    try {
        var compEstado = dijit.byId(idEstado);
        var compCidade = dijit.byId(idCidade);
        var compBairro = dijit.byId(idBairro);
        var compRua = dijit.byId(idRua);
        var compCep = dijit.byId(idCep);
        compEstado._onChangeActive = false;
        compEstado.reset();
        compEstado.oldValue = 0;
        compEstado._onChangeActive = true;
        //Cidade
        compCidade._onChangeActive = false;
        compCidade.reset();
        compCidade._onChangeActive = true;
        //bairro
        compBairro._onChangeActive = false;
        compBairro.reset();
        compBairro.oldValue = 0;
        compBairro._onChangeActive = true;
        //rua
        compRua._onChangeActive = false;
        compRua.reset();
        compRua._onChangeActive = true;
        //rua
        //Cep - Correção para não limpar o cep.
        //compCep._onChangeActive = false;
        //compCep.reset();
        //compCep._onChangeActive = true;
        dijit.byId(idCidade).set("readOnly", true)
        dijit.byId(idBairro).set("readOnly", true)
        dijit.byId(idRua).set("readOnly", true)
        //compCidade.set("disabled", true);
        //compBairro.set("disabled", true);
        //compRua.set("disabled", true);
        switch (parseInt(layout)) {
            case LAYOUTPADRAO:
                compEstado.set("readOnly", false);
                if (hasValue(btnCadCidade) && hasValue(dijit.byId(btnCadCidade)))
                    dijit.byId(btnCadCidade).set("disabled", true)
                if (hasValue(btnCadBairro) && hasValue(dijit.byId(btnCadBairro)))
                    dijit.byId(btnCadBairro).set("disabled", true)
                if (hasValue(btnCadLog) && hasValue(dijit.byId(btnCadLog)))
                    dijit.byId(btnCadLog).set("disabled", true)
                break;
            case LAYOUTPESQUISACEP:
                compEstado.set("readOnly", true);
                dijit.byId(idBairro).set("readOnly", false)
                break;
            case LAYOUTPESQUISALOGRADOURO:
                compEstado.set("readOnly", true);
                dijit.byId(idBairro).set("readOnly", false)
                break;
            case LAYOUTCEPCOMPLETO:
                compEstado.set("readOnly", true);
                if (hasValue(btnCadCidade) && hasValue(dijit.byId(btnCadCidade)))
                    dijit.byId(btnCadCidade).set("disabled", true)
                if (hasValue(btnCadBairro) && hasValue(dijit.byId(btnCadBairro)))
                    dijit.byId(btnCadBairro).set("disabled", true)
                if (hasValue(btnCadLog) && hasValue(dijit.byId(btnCadLog)))
                    dijit.byId(btnCadLog).set("disabled", true)
                break;
            default:
                compEstado.set("readOnly", false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValuesEndereco(data, idEstado, idCidade, idBairro, idRua, idCdRua, idCep, idLogradouro) {
    try {
        var compEstado = dijit.byId(idEstado);
        var compCidade = dijit.byId(idCidade);
        var compBairro = dijit.byId(idBairro);
        var compCep = dijit.byId(idCep);
        //Estado
        compEstado._onChangeActive = false;
        compEstado.set("value", data.cd_loc_estado);
        compEstado.oldValue = data.cd_loc_estado;
        compEstado._onChangeActive = true;
        //Cidade
        compCidade._onChangeActive = false;
        criarOuCarregarCompFiltering(idCidade, [{ id: data.cd_loc_cidade, name: data.noLocCidade }], "", data.cd_loc_cidade, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
        compCidade._onChangeActive = true;
        //bairro
        //compBairro._onChangeActive = false;
        if (hasValue(data.bairros)) {
            criarOuCarregarCompFiltering(idBairro, data.bairros, "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');

        } else
            criarOuCarregarCompFiltering(idBairro, [{ id: data.cd_loc_bairro, name: data.noLocBairro }], "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
        dijit.byId(idBairro).oldValue = data.cd_loc_bairro;
        //compBairro._onChangeActive = true;
        //Cep
        compCep._onChangeActive = false;
        compCep.set("value", data.num_cep);
        compCep._onChangeActive = true;
        dojo.byId(idCdRua).value = data.cd_loc_logradouro;
        dijit.byId(idRua).set("value", data.noLocRua);
        if (hasValue(data.cd_tipo_logradouro))
            dijit.byId(idLogradouro).set("value", data.cd_tipo_logradouro);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCPF(idCpf, idApreMsg) {
    try {
        var myCPF;
        myCPF = $(idCpf).val().replace('.', '').replace('.', '').replace('-', '');
        var numeros, digitos, soma, i, resultado, digitos_iguais;
        digitos_iguais = 1;

        if (myCPF.length < 11) {
            mostrarMensagenCPF(idApreMsg);
            //$("#cpf").focus();
            return false;
        }
        for (i = 0; i < myCPF.length - 1; i++)
            if (myCPF.charAt(i) != myCPF.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }
        if (!digitos_iguais) {
            numeros = myCPF.substring(0, 9);
            digitos = myCPF.substring(9);
            soma = 0;
            for (i = 10; i > 1; i--)
                soma += numeros.charAt(10 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0)) {
                mostrarMensagenCPF(idApreMsg);
                //$("#cpf").focus();
                return false;
            }
            numeros = myCPF.substring(0, 10);
            soma = 0;
            for (i = 11; i > 1; i--)
                soma += numeros.charAt(11 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1)) {
                mostrarMensagenCPF(idApreMsg);
                // $("#cpf").focus();
                return false;
            }
            return true;
        }
        else {
            mostrarMensagenCPF(idApreMsg);
            //$("#cpf").focus();
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCnpj(cnpjEmpresa, idApresMsg) {
    try {
        var cnpj;
        cnpj = $(cnpjEmpresa).val();
        cnpj = jQuery.trim(cnpj);// retira espaços em branco
        // DEIXA APENAS OS NÚMEROS
        cnpj = cnpj.replace('/', '');
        cnpj = cnpj.replace('.', '');
        cnpj = cnpj.replace('.', '');
        cnpj = cnpj.replace('-', '');

        var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
        digitos_iguais = 1;

        if (cnpj.length < 14 && cnpj.length < 15) {
            mostrarMensagenCnpj(idApresMsg);
            return false;
        }
        for (i = 0; i < cnpj.length - 1; i++) {
            if (cnpj.charAt(i) != cnpj.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }
        }

        if (!digitos_iguais) {
            tamanho = cnpj.length - 2
            numeros = cnpj.substring(0, tamanho);
            digitos = cnpj.substring(tamanho);
            soma = 0;
            pos = tamanho - 7;

            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2) {
                    pos = 9;
                }
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0)) {
                mostrarMensagenCnpj(idApresMsg);
                return false;
            }
            tamanho = tamanho + 1;
            numeros = cnpj.substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2) {
                    pos = 9;
                }
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1)) {
                mostrarMensagenCnpj(idApresMsg);
                return false;
            }
            return true;
        } else {
            mostrarMensagenCnpj(idApresMsg);
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostrarMensagenCPF(idApreMsg) {
    try {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgCPFInvalid);
        apresentaMensagem(idApreMsg, mensagensWeb);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostrarMensagenCnpj(idApresMsg) {
    try {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgCNPJInvalid);
        apresentaMensagem(idApresMsg, mensagensWeb);
    }
    catch (e) {
        postGerarLog(e);
    }
}