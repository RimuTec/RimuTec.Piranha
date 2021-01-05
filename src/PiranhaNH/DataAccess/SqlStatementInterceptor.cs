using System;
using System.Diagnostics;
using NHibernate;
using NHibernate.SqlCommand;

namespace RimuTec.PiranhaNH.DataAccess
{
   internal class SqlStatementInterceptor : EmptyInterceptor
   {
      public override SqlString OnPrepareStatement(SqlString sql)
      {
         // Idea for the implementation from
         // http://stackoverflow.com/questions/2134565/how-to-configure-fluent-nhibernate-to-output-queries-to-trace-or-debug-instead-o
         // [Manfred, 23mar2014]
         var preparedSql = base.OnPrepareStatement(sql);

         string preparedSqlAsString = preparedSql.ToString();
         if (preparedSqlAsString?.Length == 0) {
            throw new InvalidOperationException("No query translators found. Did you create a session factory including maps? You could use Database.Init(). (Code 180626-1220)");
         }

         // object obj = CallContext.GetData("QueryCounter");
         // if(obj == null) {
         //    CallContext.SetData("QueryCounter", 0);
         //    obj = CallContext.GetData("QueryCounter");
         // }
         // var queryCounterValue = (int)obj;
         // queryCounterValue++;
         // CallContext.SetData("QueryCounter", queryCounterValue);

         //Trace.WriteLine(preparedStatement);
         //Trace.WriteLine("=== SQL Statement Boundary ===");

         // Used the following commented-out code to track down expensive SQL queries [Manfred, 08apr2017]
         //var stacktrace = new StackTrace();
         //if (stacktrace.ToString().Contains("EmailNotificationService.SendInboxItemEmailNotification(")) {
         //   Trace.WriteLine(preparedStatement);
         //}
         //if (preparedStatement.ToString().Contains("SELECT users0_.RoleId as RoleI1_51_3_, users0_.UserId as UserI2_51_3_, user1_.Id as Id1_60_0_, user1_.CreatedAt as Creat2_60_0_,")) {
         //   Trace.WriteLine(preparedStatement);
         //}

         //string stmtAsString = preparedStatement.ToString();
         //if (stmtAsString.Contains("(@p2 nvarchar(4000),@p3 int,@p1 int)")
         //   || stmtAsString.Contains("Id) as __hibernate_sort_row from [JobAd] jobad")) {
         //   Trace.WriteLine(preparedStatement);
         //}

         if(preparedSqlAsString.IndexOf("UPDATE Block", StringComparison.OrdinalIgnoreCase) >= 0)
         {
            Trace.WriteLine("###############################################");
            Trace.WriteLine(preparedSqlAsString);
            Trace.WriteLine("###############################################");
         }

         return preparedSql;
      }
   }
}
