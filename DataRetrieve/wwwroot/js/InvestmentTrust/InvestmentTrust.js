"use strict";
var InvestmentTrust = InvestmentTrust || {};

(function () {
    var EditING = false;

    this.Search = function() {
        $("[name=page]").val(1)
        $("[name=vNO]").val($("[name=NO]").val())
        $("[name=vIDNO]").val($("[name=IDNO]").val())
        $("[name=vNAME]").val($("[name=NAME]").val())
        $("[type=submit]").click()
    }

    this.Delete = function(e) {
        var obj = $(e)
        var NO = $(obj).closest("tr").find(".Edit:eq(0)").val()

        swal({
            title: "確定刪除？",
            type: "question",
            showCancelButton: true//顯示取消按鈕
        }).then(
            function (result) {
                if (result.value) {
                    //使用者按下「確定」要做的事

                    $.ajax({
                        type: 'POST',
                        url: '/InvestmentTrust/Delete',
                        data: { NO: NO }, //發送參數
                        success: function (data) {
                            $("[name=page]").val(1)
                            InvestmentTrust.Search();
                            swal("群益投信客戶", "刪除" + data.MsgName + "！", "success");
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText)
                        }
                    });
                }
            });//end then 

    }
    this.Edit = function (e) {
        if (!EditING) {
            var obj = $(e)
            $(obj).closest("tr").find(".Edit:eq(1),.Edit:eq(2)").show();
            $(obj).closest("tr").find(".Show:eq(1),.Show:eq(2)").hide();
            $(obj).closest("tr").find(".btnSave").show();
            $(obj).hide();
            EditING = true;
        } else {
            alert("無法編輯")
        }
    }
    this.Save = function (e) {
        var obj = $(e)
        var NO = $(obj).closest("tr").find(".Edit:eq(0)").val()
        var NAME = $(obj).closest("tr").find(".Edit:eq(1)").val()
        var IDNO = $(obj).closest("tr").find(".Edit:eq(2)").val()

        var obj = $(obj)
        var strCheck = CheckField(obj)
        if (strCheck) {
            swal({
                title: '欄位檢核',
                type: 'info',
                html: strCheck
            })
        } else {
            swal({
                title: "確定修改？",
                type: "question",
                showCancelButton: true//顯示取消按鈕
            }).then(
                function (result) {
                    if (result.value) {
                        //使用者按下「確定」要做的事

                        $.ajax({
                            type: 'POST',
                            url: '/InvestmentTrust/Update',
                            data: { NO: NO, NAME: NAME, IDNO: IDNO }, //發送參數
                            success: function (data) {
                                InvestmentTrust.Search();
                                EditING = false;
                                //console.log(data.MsgCode.toString().toUpperCase())
                                swal("群益投信客戶", "修改" + data.MsgName + "！", data.MsgCode.toString().toUpperCase() == "TRUE" ? "success" : "error");
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText)
                            }
                        });
                    } else {
                        $(obj).closest("tr").find(".Edit:eq(1)").val($(obj).closest("tr").find(".Show:eq(1)").html())
                        $(obj).closest("tr").find(".Edit:eq(2)").val($(obj).closest("tr").find(".Show:eq(2)").html())
                        $(obj).closest("tr").find(".Edit:eq(1),.Edit:eq(2)").hide();
                        $(obj).closest("tr").find(".Show:eq(1),.Show:eq(2)").show();
                        $(obj).closest("tr").find(".btnEdit").show();
                        $(obj).hide();
                        EditING = false;
                    }
                });//end then 
        }
    }
    var CheckField = function(obj) {
        var strCheck = "";
        var Name = $(obj).closest("tr").find(".Edit:eq(1)").val();
        var IDNO = $(obj).closest("tr").find(".Edit:eq(2)").val()
        if (!Name) {
            strCheck += "[客戶名稱]為必填<br/>";
        } else if (Name.length > 50) {
            strCheck += "[客戶名稱]不可超過50個字元<br/>";
        }
        if (!IDNO) {
            strCheck += "[身份證字號或營利事業統一編號]為必填<br/>";
        } else if (IDNO.length > 10) {
            strCheck += "[身份證字號或營利事業統一編號]不可超過10個字元<br/>";
        }
        return strCheck;
    }
    this.FieldKeyUp = function (e) {
        return checkEditField();
    }
    var checkEditField = function() {
        var check = true;
        if (!$("[name=NO]").val()) {
            FieldError("NO", "[編號]為必填")
            check = check && false;
        } else if ($("[name=NO]").val().length > 3) {
            FieldError("NO", "[編號]不可超過3個字元")
            check = check && false;
        } else {
            FieldError("NO")
        }
        if (!$("[name=NAME]").val()) {
            FieldError("NAME", "[客戶名稱]為必填")
            check = check && false;
        } else if ($("[name=NAME]").val().length > 50) {
            FieldError("NAME", "[客戶名稱]不可超過50個字元")
            check = check && false;
        } else {
            FieldError("NAME")
        }
        if (!$("[name=IDNO]").val()) {
            FieldError("IDNO", "[身分證字號或營利事業統一編號]為必填")
            check = check && false;
        } else if ($("[name=IDNO]").val().length > 10) {
            FieldError("IDNO", "[身分證字號或營利事業統一編號]不可超過10個字元")
            check = check && false;
        } else {
            FieldError("IDNO")
        }
        return check;
    }
    var FieldError = function(Field, ErrorMsg) {
        if (ErrorMsg)
            $("[name=" + Field + "]").attr("aria-invalid", true).closest("div").find(".help-block-error").html(ErrorMsg).closest(".form-md-line-input").addClass("has-error");
        else
            $("[name=" + Field + "]").attr("aria-invalid", false).closest("div").find(".help-block-error").html('').closest(".form-md-line-input").removeClass("has-error");
    }
}.apply(InvestmentTrust));