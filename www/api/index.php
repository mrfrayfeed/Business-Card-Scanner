<?php
include("dbcontext.php");
$obj = new DbContext;
$obj->mysql_connect();
$id = $_GET["id"];


if ($id != "" && $id != null) {
    $data = $obj->mysql_query("SELECT * FROM `data` WHERE id = :id", array(':id' => $id), TRUE);
    if ($data != null) {
        $result = $data[0];
        $result["details"] = json_decode($data[0]["data"], true);
    } else {
        $result["error"] = "Id not found";
    }
} else {
    $result["error"] = "Missing id";
}

if ($result != null) {
    echo json_encode($result, JSON_UNESCAPED_UNICODE);
}
$obj->mysql_destroy();
