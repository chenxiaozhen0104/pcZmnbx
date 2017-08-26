var selectTime = {
    Type: {
        Day: 1,
        Week: 2,
        Month: 3,
        Year: 4
    },
    selectType: 1,
    nowDate: new Date(),
    //格式化时间
    getNowFormatDate: function () {
        var date = this.nowDate;
        var seperator1 = "-";
        var seperator2 = ":";
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
            + " " + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
        return currentdate;
    },
    //格式化开始和结束时间1开始时间,2结束时间
    getFormatDate: function (value, TimeType) {
        var date = new Date(value);
        var seperator1 = "-";
        var seperator2 = ":";
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        if (TimeType == 1)
            return date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + "00:00:00";
        else
            return date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + "23:59:59";
    },

    getFormatMonthDay: function (year, month, timeType) {
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        var day = new Date(year, month, 0);
        //获取天数：
        var daycount = day.getDate();
        if (timeType == 1)
            return year + "-" + month + "-" + "01"
                + " " + "00:00:00";
        else
            return year + "-" + month + "-" + daycount
                + " " + "23:59:59";
    },
    getFormatYearDay: function (year, timeType) {
        if (timeType == 1)
            return year + "-" + "01" + "-" + "01"
                + " " + "00:00:00";
        else
            return year + "-" + "12" + "-" + "31"
                + " " + "23:59:59";
    },
    //上一天(一周，一月)
    Previous: function () {
        //上一天
        if (this.selectType == this.Type.Day) {
            this.nowDate = this.addDate(this.nowDate, -1);
            this.BeginTime = this.getFormatDate(this.nowDate, 1);
            this.EndTime = this.getFormatDate(this.nowDate, 2);
        } else if (this.selectType == this.Type.Week) {
            this.nowDate = this.addDate(this.nowDate, -7);
            var week = new Date(this.nowDate).getDay()
            this.BeginTime = this.getFormatDate(this.addDate(this.nowDate, -week + 1), 1);
            this.EndTime = this.getFormatDate(this.addDate(this.nowDate, 7 - week), 2);
        } else if (this.selectType == this.Type.Month) {
            this.nowDate.setMonth(this.nowDate.getMonth() - 1);
            this.BeginTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 1);
            this.EndTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 2);

        } else if (this.selectType == this.Type.Year) {
            this.nowDate.setFullYear(this.nowDate.getFullYear() - 1);

            this.BeginTime = this.getFormatYearDay(this.nowDate.getFullYear(), 1);
            this.EndTime = this.getFormatYearDay(this.nowDate.getFullYear(), 2);
        }
        else {
            this.nowDate = this.addDate(this.nowDate, -1);
            this.BeginTime = this.getFormatDate(this.nowDate, 1);
            this.EndTime = this.getFormatDate(this.nowDate, 2);
        }
        return [this.BeginTime, this.EndTime];
       
    },
    Next: function () {
        if (this.selectType == this.Type.Day) {
            this.nowDate = this.addDate(this.nowDate, 1);
            this.BeginTime = this.getFormatDate(this.nowDate, 1);
            this.EndTime = this.getFormatDate(this.nowDate, 2);

        } else if (this.selectType == this.Type.Week) {
            this.nowDate = this.addDate(this.nowDate, 7);
            var week = new Date(this.nowDate).getDay()
            this.BeginTime = this.getFormatDate(this.addDate(this.nowDate, -week + 1), 1);
            this.EndTime = this.getFormatDate(this.addDate(this.nowDate, 7 - week), 2);

        } else if (this.selectType == this.Type.Month) {
            this.nowDate.setMonth(this.nowDate.getMonth() + 1);
            this.BeginTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 1);
            this.EndTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 2);
        }
        else if (this.selectType == this.Type.Year) {
            this.nowDate.setFullYear(this.nowDate.getFullYear() + 1);
            this.BeginTime = this.getFormatYearDay(this.nowDate.getFullYear(), 1);
            this.EndTime = this.getFormatYearDay(this.nowDate.getFullYear(), 2);

        }
        return [this.BeginTime, this.EndTime];
       
    },
    BeginTime: '',
    EndTime: '',
    setTimeType: function (type) {
        this.selectType = type;
        this.nowDate = new Date();
        if (type == this.Type.Day) {
            this.nowDate = this.addDate(this.nowDate, -0);
            this.BeginTime = this.getFormatDate(this.nowDate, 1);
            this.EndTime = this.getFormatDate(this.nowDate, 2)
        } else if (type == this.Type.Week) {
            var week = this.nowDate.getDay();
            this.BeginTime = this.getFormatDate(this.addDate(this.nowDate, -week + 1), 1);
            this.EndTime = this.getFormatDate(this.addDate(this.nowDate, 7 - week), 2);

        } else if (type == this.Type.Month) {
            this.BeginTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 1);
            this.EndTime = this.getFormatMonthDay(this.nowDate.getFullYear(), this.nowDate.getMonth() + 1, 2);
        }
        else if (type == this.Type.Year) {
            this.BeginTime = this.getFormatYearDay(this.nowDate.getFullYear(), 1);
            this.EndTime = this.getFormatYearDay(this.nowDate.getFullYear(), 2);
        }
    
        return dd = [this.BeginTime, this.EndTime];
       
    },
    addDate: function (value, day) {
        var a = new Date(value)
        a = a.valueOf()
        a = a + day * 24 * 60 * 60 * 1000
        a = new Date(a)
        return a;
    }

};
