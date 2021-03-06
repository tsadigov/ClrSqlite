﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    using Ast;
    public static class SelectExtensions
    {
        public static void heightOfSelect(this Select _this,ref int pnHeight)
        {
            if (_this != null)
            {
                _this.pWhere.heightOfExpr(ref pnHeight);
                _this.pHaving.heightOfExpr(ref pnHeight);
                _this.pLimit.heightOfExpr(ref pnHeight);
                _this.pOffset.heightOfExpr(ref pnHeight);
                _this.ResultingFieldList.heightOfExprList(ref pnHeight);
                _this.pGroupBy.heightOfExprList(ref pnHeight);
                _this.pOrderBy.heightOfExprList(ref pnHeight);
                _this.pPrior.heightOfSelect(ref pnHeight);
            }
        }
    }
}
