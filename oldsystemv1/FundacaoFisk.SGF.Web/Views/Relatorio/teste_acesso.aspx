<!DOCTYPE html>
<html >
	<head>

		<link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/dojo/1.8.10/dijit/themes/claro/claro.css" />
		<script src="http://ajax.googleapis.com/ajax/libs/dojo/1.8.10/dojo/dojo.js" data-dojo-config="async: true, isDebug: true, parseOnLoad: true, locale: 'en-us'"></script>
		
		<script>
	require(["dojo/parser", "dijit/form/DateTextBox"]);
	function acessaUrl(){
		require(["dojo/_base/xhr"], function (xhr) {
			xhr.get({
				url: "http://www.sgf.datawin.com.br/api/coordenacao/getAvaliacaoCriterio?cd_tipo_avaliacao=0&cd_criterio_avaliacao=0",
				headers: { "X-Requested-With": null }
			}).then(function (dataCriterio) {
				alert(dataCriterio);
			},
			function (error) {
				apresentaMensagem('apresentadorMensagemCurso', error);
			});
		});
	}
		</script>
	</head>
	<body class="claro">
		<!DOCTYPE html>
		<html >
			<head>

				<link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/dojo/1.8.10/dijit/themes/claro/claro.css" />
				<script src="http://ajax.googleapis.com/ajax/libs/dojo/1.9.1/dojo/dojo.js" data-dojo-config="async: true, isDebug: true, parseOnLoad: true, locale: 'en-us'"></script>
				
				<script>
			require(["dojo/parser", "dijit/form/DateTextBox"]);
				</script>
			</head>
			<body class="claro">
				
				<button type="button" onclick="acessaUrl();"/>
			</body>
		</html>
		<p>teste</p>
	</body>
</html>