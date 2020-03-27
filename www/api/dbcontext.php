<?php

class DbContext
{
    private $host = "localhost";
    private $dbname = "";
    private $user = "";
    private $pass = "";
    private $charset = "utf8";

    private $pdo;


    public function mysql_connect()
    {
        date_default_timezone_set('Europe/Kiev');
        $dsn = "mysql:host=" . $this->host . ";dbname=" . $this->dbname . ";charset=" . $this->charset;
        $connopt = array(
            PDO::ATTR_ERRMODE  => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC
        );
        $this->pdo = new PDO($dsn, $this->user, $this->pass, $connopt);
    }

    public function mysql_get_status()
    {
        if (is_null($this->pdo)) {
            return false;
        } elseif (
            $this->pdo->getAttribute(PDO::ATTR_CONNECTION_STATUS) === $this->host . " via TCP/IP"
        ) {
            return true;
        } else {
            return false;
        }
    }

    public function mysql_query($query, $placeholders = null, $select = true)
    {
        if ($this->mysql_get_status()) {
            $stmt = $this->pdo->prepare($query);
            if (!is_null($placeholders)) {
                $stmt->execute($placeholders);
            } else {
                $stmt->execute();
            }
            if ($select) {
                $arr = $stmt->fetchAll();
                return $arr;
            }
        } else {
            return false;
        }
    }

    public function mysql_destroy()
    {
        $this->pdo = null;
    }
}
