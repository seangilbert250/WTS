using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WTS.Auditing
{
    public class Auditing
    {
        public Auditing()
        {

        }

        public static bool AuditLog_Save(int ItemID, int ParentItemID, int AuditLogTypeID, int ITEM_UPDATETYPEID, string FieldChanged, string OldValue, string NewValue, DateTime UpdatedDate, string UpdatedBy)
        {
            if (OldValue == NewValue)
            {
                return true;
            }

            return WTSData.ExecuteStoredProcedure("AuditLog_Save", new SqlParameter[] {
                new SqlParameter("@ItemID", ItemID),
                new SqlParameter("@ParentItemID", (ParentItemID > 0 ? ParentItemID : (object)DBNull.Value)),
                new SqlParameter("@AuditLogTypeID", AuditLogTypeID),
                new SqlParameter("@ITEM_UPDATETYPEID", ITEM_UPDATETYPEID),
                new SqlParameter("@FieldChanged", FieldChanged),
                new SqlParameter("@OldValue", string.IsNullOrWhiteSpace(OldValue) ? (object)DBNull.Value : OldValue),
                new SqlParameter("@NewValue", string.IsNullOrWhiteSpace(NewValue) ? (object)DBNull.Value : NewValue),
                new SqlParameter("@UpdatedDate", UpdatedDate == DateTime.MinValue ? (object)DBNull.Value : UpdatedDate),
                new SqlParameter("@UpdatedBy", UpdatedBy)
            });
        }

        public static DataTable AuditHistory_Get(int AuditLogTypeID, int ItemID, int ParentItemID, DateTime maxUpdatedDate)
        {
            return WTSData.GetDataTableFromStoredProcedure("AuditHistory_Get", null, new SqlParameter[] {
                new SqlParameter("@AuditLogTypeID", AuditLogTypeID),
                new SqlParameter("@ItemID", ItemID),
                new SqlParameter("@ParentItemID", ParentItemID > 0 ? ParentItemID : (object)DBNull.Value),
                new SqlParameter("@MaxUpdatedDate", maxUpdatedDate != DateTime.MinValue ? maxUpdatedDate : (object)DBNull.Value)
            });
        }
    }
}