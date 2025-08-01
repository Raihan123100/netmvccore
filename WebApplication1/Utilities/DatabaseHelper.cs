using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Utilities
{
    public class DatabaseHelper
    {

        private readonly string _connectionString;
        //private readonly string _connectionString;
        private OracleCommand _command;
        private OracleConnection _connection;
        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        public DataTable ExecuteQuery(string query)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand(query, conn))
                {
                    var dt = new DataTable();
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    return dt;
                }
            }
        }



        public void CallAddIncentive(int employeeid, string branchcode, decimal teamkpi)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("incentive_pkg.add_incentive", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_employeeid", OracleDbType.Int32).Value = employeeid;
                cmd.Parameters.Add("p_branchcode", OracleDbType.Varchar2).Value = branchcode;
                cmd.Parameters.Add("p_teamkpi", OracleDbType.Decimal).Value = teamkpi;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }





        public bool Exists(string query, Dictionary<string, object> parameters)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand(query, conn))
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param.Key, param.Value);
                }

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null;
            }
        }





        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param.Key, param.Value);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void CreditAccount(string accountNo, decimal amount)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("account_pkg.credit_account", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_account_no", OracleDbType.Varchar2).Value = accountNo;
                cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DebitAccount(string accountNo, decimal amount)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("account_pkg.debit_account", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_account_no", OracleDbType.Varchar2).Value = accountNo;
                cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public decimal GetAccountBalance(string accountNo)
        {
            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("SELECT balance FROM bank_accounts WHERE account_no = :accountNo", conn))
            {
                cmd.Parameters.Add("accountNo", OracleDbType.Varchar2).Value = accountNo;
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }

        public List<TransactionLog> GetTransactionHistory(string accountNo)
        {
            var transactions = new List<TransactionLog>();

            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("SELECT txn_type, amount, txn_time FROM transaction_log WHERE account_no = :accountNo ORDER BY txn_time DESC FETCH FIRST 10 ROWS ONLY", conn))
            {
                cmd.Parameters.Add("accountNo", OracleDbType.Varchar2).Value = accountNo;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new TransactionLog
                        {
                            Type = reader.GetString(0),
                            Amount = reader.GetDecimal(1),
                            Time = reader.GetDateTime(2)
                        });
                    }
                }
            }

            return transactions;
        }



        public void SetParameters(OracleParameter[] parameters)
        {
            if (_command == null)
            {
                throw new InvalidOperationException("Command not initialized. Call CreateCommand first.");
            }

            _command.Parameters.Clear();
            foreach (var param in parameters)
            {
                _command.Parameters.Add(param);
            }
        }
        public void CreateCommand(string sql, CommandType commandType)
        {
            _connection = new OracleConnection(_connectionString);
            _command = new OracleCommand(sql, _connection)
            {
                CommandType = commandType,
                BindByName = true // Important for Oracle parameter binding
            };
        }

        public OracleDataReader ExecuteReader()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public OracleDataReader ExecuteReader(string sql, CommandType commandType, OracleParameter[] parameters = null)
        {
            CreateCommand(sql, commandType);
            if (parameters != null)
            {
                SetParameters(parameters);
            }
            return ExecuteReader();
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        public void Dispose()
        {
            _command?.Dispose();
            _connection?.Dispose();
        }
    }
}

