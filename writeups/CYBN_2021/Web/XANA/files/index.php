<?php

require __DIR__ . '/vendor/autoload.php';

use Firebase\JWT\JWT;
use Firebase\JWT\Key;

class Lyoko {
	public $key;
	public $xana;
	public $world_init;
	public $isHacked;

	public function __construct() {
		$this->xana = getenv('FLAG');
		$this->key = getenv('XANA_KEY');
		$this->world_shutdown = new DateTime(date("Y-m-d H:i:s", strtotime("3 December 2021 8:00")));
		$this->world_init = clone $this->world_shutdown;
		$this->world_init->modify('-24 hours');
		$this->identity = $this->get_identity();
		$this->isHacked = $this->identity["iat"] > $this->world_init->getTimestamp();

	}

	public function get_identity() {
		try {
			if (!isset($_COOKIE["lyokoToken"])) {
				$payload = $this->generate_identity();
			} else {
				$payload = $_COOKIE["lyokoToken"];
			}

			$jwt = (array) JWT::decode($payload, new Key($this->key, "HS256"));
		} catch (Exception $e) {
			echo $e;
			die('XANA CANNOT BE HACKED');
		}
		return $jwt;
	}

	public function generate_identity() {
		$payload = array(
			"iat" => time(),
		);
		$jwt = JWT::encode($payload, $this->key, "HS256");
		setcookie("lyokoToken", "$jwt");
		return $jwt;
	}
}


$Lyoko = new Lyoko();
?>

<!DOCTYPE HTML>
<html>
<?php if (!$Lyoko->isHacked): ?>
	<head>
		<title>Welcome to Lyoko</title>
		<style>
			body {
				background-color: darkblue;
				color: white;
				font-family: 'Courier New', sans-serif;
			}
		</style>
	</head>
	<body>
		Retour vers le passé ! XANA a été repoussé, et le monde est sauvé... pour l'instant.<br>
		<?= $Lyoko->xana; ?>
	</body>
<?php else: ?>
	<head>
		<title>HACKED BY XANA</title>
		<style>
			@import url('http://fonts.cdnfonts.com/css/gunship');
			body {
				background-color: black;
				background-image: url('xana.jpg');
				background-position: top;
				background-repeat: no-repeat;
				color: red;
				font-family: 'Gunship', sans-serif;
			}
		</style>
		<script src="http://cdn.jsdelivr.net/particles.js/2.0.0/particles.min.js"></script>
	</head>
	<body>
		<div id="particles-js">
			_ HACKED BY XANA<br>
			_ World initialized at : <?= $Lyoko->world_init->format("Y-m-d\TH:i:s.u"); ?><br>
			_ World shutdown at : <?= $Lyoko->world_shutdown->format("Y-m-d\TH:i:s.u"); ?><br>
			_ Warrior initialized at : <?= date("Y-m-d\TH:i:s.u", $Lyoko->identity["iat"]); ?>
		</div>
		<script src="particles.js"></script>
	</body>
<?php endif; ?>
</html>
