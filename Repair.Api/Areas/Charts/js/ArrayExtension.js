//扩展数组方法：查找指定元素的下标  

Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};
//扩展数组方法:删除指定元素
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    while (index > -1) {
        return this.splice(index, 1);
        
    }
};  
//数组匹配
Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}  

/** 
*   获取数组最大值 
*/
Array.prototype.max = function () {
    var i, max = this[0];
    for (i = 1; i < this.length; i++) {
        if (max < this[i])
            max = this[i];
    }
    return max;
}

/** 
*   获取数组最小值 
*/
Array.prototype.min = function () {
    var i, min = this[0];
    for (i = 1; i < this.length; i++) {
        if (min > this[i])
            min = this[i];
    }
    return min;
}
/**
* 数组去重复,返回字符串
*/
Array.prototype.unique = function () {
    var res, hash = {};
    for (var i = 0, elem; (elem = this[i]) != null; i++) {
        if (!hash[elem]) {
            if (elem) {
                res = (res ? res + elem + ',' : elem + ",");
            }

            hash[elem] = true;
        }
    }
    return res;
}

/** 
*   得到字符串索引数组
*/
String.prototype.indexOfAryAny = function (ary) {
    var that = this;
    var array = [];
    for (var i = 0; i < ary.length; i++) {
        if (that.indexOf(ary[i]) >= 0) {
            array.push(that.indexOf(ary[i]));
        }
    }
    return array;
}

/** 
*   得到字符串索引数组
*/
String.prototype.lastIndexOfAryAny = function (ary) {
    var that = this;
    var array = [];
    for (var i = 0; i < ary.length; i++) {
        if (that.lastIndexOf(ary[i]) >= 0) {
            array.push(that.lastIndexOf(ary[i]));
        }
    }
    return array;
}
