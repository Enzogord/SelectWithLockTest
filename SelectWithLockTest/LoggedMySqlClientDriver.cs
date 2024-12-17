using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using NHibernate.Driver.MySqlConnector;
using NLog;

namespace QS.Project.DB
{
	public class LoggedMySqlClientDriver : MySqlConnectorDriver 
	{
		private readonly DbType[] typesWithQuotes = { DbType.Date, DbType.DateTime, DbType.String };
		
		protected override void OnBeforePrepare(DbCommand command)
		{
			base.OnBeforePrepare(command);
			try
			{
				var text = GetResultQueryText(command);
                Console.WriteLine(text);
			}
			catch (Exception ex) {
                Console.WriteLine($"Ошибка при формировании текста SQL запроса для логгера: {ex.Message}");
			}
		}
		
		public string GetResultQueryText(DbCommand dbCommand)
		{
			string parametersText = $"{Environment.NewLine}";
			
			foreach (DbParameter parameter in dbCommand.Parameters) {
				string parameterValue;

				if(parameter.Value != DBNull.Value && (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime)) {
					parameterValue = ((DateTime)parameter.Value).ToString("s", CultureInfo.CreateSpecificCulture("ru-RU"));
				}
				else {
					parameterValue = parameter.Value.ToString();
				}
				parameterValue = typesWithQuotes.Contains(parameter.DbType) ? "'" + parameterValue + "'" : parameterValue;
				parametersText += $"SET {parameter.ParameterName.TrimStart('?')} = {parameterValue};{Environment.NewLine}";
			}
			return $"{parametersText}{dbCommand.CommandText.Replace("?p", "@p")};";
		}
	}
}
