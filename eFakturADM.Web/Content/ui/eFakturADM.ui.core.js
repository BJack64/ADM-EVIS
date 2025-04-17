var App = new function () {

    var _baseURL = "";
    return {
        getParameterByName: function (name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href);
            if (results == null)
                return "";
            else
                return decodeURIComponent(results[1].replace(/\+/g, " "));
        },

        BaseURL: function (url) {
            _baseURL = url;
        },

        InitExpander: function (slicePoint) {
            if (slicePoint == undefined)
                slicePoint = 67;

            $("div.expander").expander({
                slicePoint: slicePoint,
                widow: 0,
                expandEffect: 'show',
                expandText: "<span style=\"font-size:11px;white-space:nowrap\"> <i class=\" icon-zoom-in\"></i>Read More</span>",
                userCollapseText: '<span style=\"font-size:11px;white-space:nowrap\"> <i class=\"icon-zoom-out\"></i>Collapse</span>'
            });
        },

        InitDatePicker: function () {
            $('.datepicker').datepicker({
                onSelect: function () {
                    $(this).change();

                },
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                autoclose: true,
                todayBtn: "linked",
                todayHighlight: true,
                orientation: "auto"
            });
        },

        PreventDoubleSubmit: function () {
            $('body').append('<div id="ajaxBusy" class="ajaxBusy" style="display:none"></div>');

            window.onerror = function () {
                $('#ajaxBusy').hide();
            };
            $(document).ajaxStart(function () {
                $('#ajaxBusy').show();
            });
            $(document).ajaxStop(function () {
                $('#ajaxBusy').hide();
            });

            $('body').on('click', '.btn', function () {
                var btn = $(this);
                btn.attr("disabled", "disabled");
                setTimeout(function () {
                    btn.removeAttr("disabled");
                }, 2000);
            });
        },

        DelayButton: function () {
            var button = $(".btn-delay");
            button.attr("disabled", "disabled");
            setTimeout(function () {
                button.removeAttr("disabled");
            }, 2000);
        },

        RefreshTable: function (tableId, urlData) {
            table = $(tableId).DataTable();
            table.ajax.reload(null, false);
        },


        // Will be implemented in next release.
        SessionRequest: function () {
            //var captcha = $("#error-message-dialog").attr("class").replace("message-dialog-", "");

            return {
                RequestID: 100, //(parseInt(captcha.substr(3, 3)) + parseInt(captcha.substr(2, 7))).toString(),
                SessionID: 200 //captcha.substr(3, 1) + captcha.substr(9, 1) + captcha.substr(4, 1) + captcha.substr(6, 1) + captcha.substr(2, 1) + captcha.substr(1, 1) + captcha.substr(5, 1) + captcha.substr(8, 1) + captcha.substr(0, 1) + captcha.substr(7, 1)
            };
        },

        DialogGet: function (url, e, params) {
            if (e.preventDefault) {
                e.preventDefault();
            }

            var _this = $(this)
            var _params = {};
            var _btn = e.currentTarget;

            $(_btn).button('loading')

            if (params != undefined && params != null)
                _params = params;

            _this.attr("disabled", "disabled");

            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(_params),
                cache: false,
                success: function (data) {
                    if (data != null && data.Html)
                        $(data.Html).prependTo('body');

                    $(_this).removeAttr("disabled");
                    $(_btn).button('reset');
                },
                error: function (xhr, textStatus, errorThrown) {
                    App.DialogError(xhr, textStatus, errorThrown);
                    $(_this).removeAttr("disabled");
                    $(_btn).button('reset');
                }
            });
        },

        DialogBootstrap: function (templateId, width, height, modal, buttons, remove, onShow) {
            var realHeight = height;
            var _height = $(window).height() - 150;

            if (remove == undefined)
                remove = true;

            if (height > _height)
                height = _height;

            var dialog = null;


            var dialogButtons = [];

            $.each(buttons, function (index, type) {
                dialogButtons.push({
                    label: index,
                    cssClass: 'btn-primary btn-sm ' + index.replace(' ', ''),
                    action: function (dialog) {
                        var $button = this; // 'this' here is a jQuery object that wrapping the <button> DOM element.
                        $button.disable();
                        type();
                        setTimeout(function () {
                            $button.enable();
                        }, 3000);

                    }
                });
            });

            dialogButtons.push({
                label: 'Close',
                cssClass: 'btn-default btn-sm',
                action: function (dialogRef) {

                    if (remove)
                        $(templateId).remove();

                    dialogRef.close();
                }
            });

            var dialogId = templateId.replace('-view-', '-');
            var dialogContent = $(templateId).html().replace(/\n/g, '');
            // Using chain callings
            var dialog = new BootstrapDialog(
                    {
                        id: templateId.replace('#', '').replace('-view-', '-'),
                        buttons: dialogButtons,
                        draggable: true,
                        closable: true,
                        onshown: onShow
                    })
                .setTitle($(templateId).attr('data-dialog-tittle') + ' <div id="' + $(templateId).attr('data-dialog-spinner') + '" style="display:inline-block;height:17px;width:17px"></div>')
                .setMessage(dialogContent)
                .setType(BootstrapDialog.TYPE_DEFAULT)
                .setSize(BootstrapDialog.SIZE_WIDE);

            dialog.open();
            $('.modal-backdrop').css('background-color', '#aaa');
            // http://stackoverflow.com/questions/10636667/bootstrap-modal-appearing-under-background
            // http://coolestguidesontheplanet.com/bootstrap/modal.php
            // http://stackoverflow.com/questions/3448813/jqueryui-how-to-make-a-shadow-around-a-dialog-box
            // box-shadow: 0 0 0 7px rgba(0,0,0,0.2); 
            // http://stackoverflow.com/questions/19891165/bootstrap-custom-modal-effect
            // http://jsfiddle.net/uqR3s/
            // http://tympanus.net/Development/ModalWindowEffects/
            // http://stackoverflow.com/questions/13465221/is-it-possible-to-make-a-twitter-bootstrap-modal-slide-from-the-side-or-bottom-i
            // http://www.jqueryscript.net/demo/Responsive-jQuery-Window-Modal-System-For-Bootstrap-Bootstrap-Window/
            // http://stackoverflow.com/questions/15881245/draggable-js-bootstrap-modal-performance-issues
            setTimeout(function () {
                $(dialogId + ' .modal-body').css('overflow-y', "auto");
                $(dialogId + ' .modal-body').css('height', height);
                $(dialogId + ' .modal-body').css('max-height', 700);
                $(dialogId + ' .modal-body').css('top', "50%");
                $(dialogId + ' .modal-body').attr("id", templateId.replace('#', '').replace('-view-', '-content-'));
                $(dialogId + ' .modal-dialog').css('z-index', "15000");
                $(dialogId + ' .modal-dialog').css('height', "90%");
                $(dialogId + ' .modal-backdrop').css('position', "fixed");
                $(dialogId + ' .modal-backdrop').css('bottom', "0");
                $('body').removeClass('modal-open');
                $('body').addClass('stop-scrolling');



                if (width > 0) {
                    $(dialogId + '.modal-body').css('min-width', width + 'px');
                }

                //if (height > 0) {
                //    $(dialogId + '.modal-body').css('min-height', height + 'px');
                //}

                $(dialogId + ' .modal-content').css('webkit-box-shadow', '');
                $(dialogId + ' .modal-content').css('box-shadow', '0 0 0 8px rgba(0,0,0,0.15)');
                $(dialogId + ' .modal-content').css('width', width);
                $(dialogId + ' .modal-content').css('left', "50%");
                $(dialogId + ' .modal-content').css('margin-left', -(width / 2));
                if (height != "auto") {
                    //if ($(window).width() > 1366) {
                    //    $(dialogId + ' .modal-content').css('top', "50%");
                    //    $(dialogId + ' .modal-content').css('margin-top', -((height / 2) + 150));
                    //}
                    //else {
                        $(dialogId + ' .modal-content').css('top', "50%");
                        $(dialogId + ' .modal-content').css('margin-top', -((realHeight / 2)));
                    //}
                }

            }, 160);

            return dialog;

        },

        Dialog: function (dialogId, width, height, modal, buttons, remove) {

            var _height = $(window).height() - 150;

            if (remove == undefined)
                remove = true;

            if (height > _height)
                height = _height;

            var dialog = $(dialogId).dialog({
                hide: { effect: "fade", duration: 275 },
                show: { effect: "fade", duration: 225 }, // 225
                resizable: true,
                height: height, //$(window).height() - 250,
                width: width,
                modal: modal,
                close: function (event, ui) { if (remove) { $(dialogId).remove(); }; },
                buttons: buttons
            });

            // Remove focus from buttons
            $(dialog).parent().find('.ui-dialog :button').blur();

            $(dialog).parent().find("button")
                .addClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
                .attr("aria-disabled", "false")
                .attr("role", "button");

            // Buttons
            $('div.ui-dialog').each(function () { $(this).css('overflow', 'visible'); });
            $('div.ui-dialog-container').each(function () { $(this).css('overflow', 'hidden'); });

            $(dialogId).dialog('open');
            $(dialogId).parent().css("border", "none");
            $(dialogId).parent().css("padding", "0px");
            $(dialogId).parent().css("padding-right", "3px");
            $(dialogId).parent().css("margin-top", "25px");

            return dialog;
        },

        DialogError: function (xhr, textStatus, errorThrown) {

            try {
                var data = jQuery.parseJSON('{' + xhr.responseText + '"}')
                if (data.redirect != null) {
                    App.SessionWasExpiredDialog(data.redirect);
                    return;
                }
            } catch (e) { }


            var iFrame = "<iframe id=\"issue-frame\" src=\"\\Errors\\ErrorMessage\" style=\"border:none;height:500px;width:100%\"></iframe>";

            var dialogButtons = [];

            dialogButtons.push({
                label: 'Close',
                cssClass: 'btn-default btn-sm',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            });

            // Using chain callings
            var dialog = new BootstrapDialog(
                    {

                        buttons: dialogButtons,
                        draggable: true,
                        closable: true
                    })
                .setMessage(iFrame)
                .setType(BootstrapDialog.TYPE_DANGER)
                .setSize(BootstrapDialog.SIZE_WIDE);

            dialog.open();


            setTimeout(function () {
                $(' .modal-dialog').css('z-index', "15000");
                $(' .modal-body').css('overflow-y', "auto");

                $(' .modal-content').css('webkit-box-shadow', '');
                $(' .modal-content').css('box-shadow', '0 0 0 8px rgba(0,0,0,0.15)');

                //$("#issue-frame").contents().find("body").html("");

                //                if (xhr.responseText != "")
                //                    $("#issue-frame").contents().find("body").html("&nbsp;" + xhr.responseText);
                //                else
                //                    $("#issue-frame").contents().find("body").html(textStatus + ": " + errorThrown);

                $("#issue-frame").attr("display", "block");

            }, 150);


            return dialog;
        },

        DialogErrorOld: function (xhr, textStatus, errorThrown) {

            try {
                var data = jQuery.parseJSON('{' + xhr.responseText + '"}')
                if (data.redirect != null) {
                    App.SessionWasExpiredDialog(data.redirect);
                    return;
                }
            } catch (e) { }

            var dialogId = "#error-message-dialog";
            var dialogConetnt = "<div id=\"error-message-dialog\" title=\"Something went wrong\" style=\"padding:1px;overflow:hidden\"><iframe id=\"issue-frame\" src=\"\" style=\"border:none;width:100%;height:100%\"></iframe></div>";
            $(dialogConetnt).prependTo('body');

            var dialog = $(dialogId).dialog({
                hide: { effect: "fade", duration: 275 },
                show: { effect: "fade", duration: 225 }, // 225
                resizable: true,
                height: 500,
                width: 640,
                modal: true,
                close: function (event, ui) { $(dialogId).remove(); },
                buttons: {
                    Close: function () { $(this).dialog("close"); }
                },

                open: function () {
                    setTimeout(function () {
                        $("#issue-frame").contents().find("body").html("");

                        if (xhr.responseText != "")
                            $("#issue-frame").contents().find("body").html("&nbsp;" + xhr.responseText);
                        else
                            $("#issue-frame").contents().find("body").html(textStatus + ": " + errorThrown);

                        $("#issue-frame").attr("display", "block");
                    }, 2);
                }
            });

            // Remove focus from buttons
            $(dialog).parent().find('.ui-dialog :button').blur();

            $(dialog).parent().find("button")
                .addClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
                .attr("aria-disabled", "false")
                .attr("role", "button");

            // Buttons
            $('div.ui-dialog').each(function () { $(this).css('overflow', 'visible'); });
            $('div.ui-dialog-container').each(function () { $(this).css('overflow', 'hidden'); });

            $(dialogId).dialog('open');
            $(dialogId).parent().css("border", "none");
            $(dialogId).parent().css("padding", "0px")
            return dialog;
        },

        SessionWasExpiredDialog: function (url) {

            $('<div id="session-time-out" style="display:none;overflow-y:auto" title="Session Was Expired"> <p>For some reason your session was expired. You should click on "Sign In" button to be back to the application.</p> </div>').prependTo('body');

            App.Dialog("#session-time-out", 450, 170, true,
                { "Sign In": function (e) { top.location.href = url; } },
                true);
        },

        ResetAvavtarImage: function (url, memberid) {
            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ MemberID: memberid }),
                cache: false,
                success: function (data) {
                    switch (data.NotifyType) {
                        case 0: $("#general-message").html(data.Html);; break
                        case 1: $("#avatar-notify-message").html(data.Html);; break
                        case 2: $(data.Html).prependTo('body'); break;
                    };

                    setTimeout(function (e) {
                        if (data.NotifyType == undefined) {
                            $("#member-avatar").attr("src", data.FileURL);
                        }
                    }, 100);

                },
                error: function (xhr, textStatus, errorThrown) {
                    App.DialogError(xhr, textStatus, errorThrown);
                }
            });
        },

        ResetAvavtarUploadedImage: function (url) {
            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ id: 0 }),
                cache: false,
                success: function (data) {

                },
                error: function (xhr, textStatus, errorThrown) {

                }
            });
        },

        InitAvatarUpload: function (btn, url, name, action, action2, memberid) {

            var button = btn;

            new AjaxUpload(name, {
                action: action,
                data: { MemberID: memberid },
                onSubmit: function (file, ext) {

                    // Allow only images. You should add security check on the server-side.
                    if (ext && /^(jpg|png|jpeg|gif|bmp)$/.test(ext.toLowerCase())) {
                        /* Setting data */
                        this.setData({
                            MemberID: memberid
                        });
                        $("#avatr-uploading-in-progess").css("display", "inline-block");
                        $("#upload-avatar").attr("disabled", "disabled");
                        $("#reset-avatar").attr("disabled", "disabled");
                    } else {
                        // extension is not allowed
                        alert($("#avatar-panel-warning-message").val());
                        // cancel upload
                        return false;
                    }

                },
                onComplete: function (file, response) {
                    $("#avatr-uploading-in-progess").css("display", "none");
                    $("#upload-avatar").removeAttr("disabled", "");
                    $("#reset-avatar").removeAttr("disabled", "");
                    var _data = JSON.parse(response);
                    if (_data.Message) {
                        $("#general-message").html(_data.Message)
                    }
                    else {
                        App.DialogGet(action2, button, { id: _data.MemberID, name: _data.Name });
                    }
                }
            });

        },

        unblockRegion: function (Name) {
            $('#' + Name).unblock();
        },

        blockRegion: function (Name) {
            $('#' + Name).block({
                //theme: true,
                //title: "", 
                message: "<div style=\"font-size:13px;height:30px;backroubd-color:white;padding-top:10px;\" class=\"block-message-region\"><b>Please wait ...</b></div>",
                overlayCSS: {
                    backgroundColor: '#000', opacity: .2,
                    "-moz-border-radius-topleft": "7px", "-webkit-border-top-left-radius": "7px", "border-top-left-radius": "7px",
                    "-moz-border-radius-topright": "7px", "-webkit-border-top-right-radius": "7px", "border-top-right-radius": "7px",
                    "-moz-border-radius-bottomright": "7px", "-webkit-border-bottom-right-radius": "7px", "border-bottom-right-radius": "7px",
                    "-moz-border-radius-bottomleft": "7px", "-webkit-border-bottom-left-radius": "7px", "border-bottom-left-radius": "7px"
                },
                css: {
                    border: '1px solid white',
                    padding: '2px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '7px',
                    '-moz-border-radius': '7px',
                    '-webkit-border-radius': '7px',
                    '-moz-border-radius': '7px',
                    '-moz-border-radius-topleft': '7px',
                    '-webkit-border-top-left-radius': '7px',
                    'border-top-left-radius': '7px',
                    '-moz-border-radius-topright': '7px',
                    '-webkit-border-top-right-radius': '7px',
                    'border-top-right-radius': '7px',
                    '-moz-border-radius-bottomright': '7px',
                    '-webkit-border-bottom-right-radius': '7px',
                    'border-bottom-right-radius': '7px',
                    '-moz-border-radius-bottomleft': '7px',
                    '-webkit-border-bottom-left-radius': '7px',
                    'border-bottom-left-radius': '7px',
                    opacity: .6,
                    color: '#fff'
                }
            });
        },

        blockRegion2: function (Name) {
            $('#' + Name).block({
                //theme: true,
                //title: "",
                message: "", // <img src='../../images/ajax-loader.gif>
                overlayCSS: {
                    backgroundColor: '#000', opacity: .2,
                    "-moz-border-radius-topleft": "7px", "-webkit-border-top-left-radius": "7px", "border-top-left-radius": "7px",
                    "-moz-border-radius-topright": "7px", "-webkit-border-top-right-radius": "7px", "border-top-right-radius": "7px",
                    "-moz-border-radius-bottomright": "7px", "-webkit-border-bottom-right-radius": "7px", "border-bottom-right-radius": "7px",
                    "-moz-border-radius-bottomleft": "7px", "-webkit-border-bottom-left-radius": "7px", "border-bottom-left-radius": "7px"
                },
                css: {
                    border: 'none',
                    padding: '2px',
                    backgroundColor: 'transparent',
                    '-webkit-border-radius': '7px',
                    '-moz-border-radius': '7px',
                    opacity: .7,
                    color: '#fff'
                }
            });
        },

        blockRegion3: function (element) {
            element.block({
                //theme: true,
                //title: "",
                message: "", // <img src='../../images/ajax-loader.gif>
                overlayCSS: {
                    backgroundColor: '#000', opacity: .2,
                    "-moz-border-radius-topleft": "4px", "-webkit-border-top-left-radius": "4px", "border-top-left-radius": "4px",
                    "-moz-border-radius-topright": "4px", "-webkit-border-top-right-radius": "4px", "border-top-right-radius": "4px",
                    "-moz-border-radius-bottomright": "4px", "-webkit-border-bottom-right-radius": "4px", "border-bottom-right-radius": "4px",
                    "-moz-border-radius-bottomleft": "4px", "-webkit-border-bottom-left-radius": "4px", "border-bottom-left-radius": "4px"
                },
                css: {
                    border: 'none',
                    padding: '2px',
                    backgroundColor: 'transparent',
                    '-webkit-border-radius': '4px',
                    '-moz-border-radius': '4px',
                    opacity: .7,
                    color: '#fff'
                }
            });
        }
    }
};




$(function () {

    // fix sub nav on scroll
    var $win = $(window)
      , $nav = $('.subnav')
      , navTop = $('.subnav').length && $('.subnav').offset().top - 40
      , isFixed = 0

    processScroll()

    $win.on('scroll', processScroll)

    function processScroll() {
        var i, scrollTop = $win.scrollTop()
        if (scrollTop >= navTop && !isFixed) {
            isFixed = 1
            $nav.addClass('subnav-fixed')
        } else if (scrollTop <= navTop && isFixed) {
            isFixed = 0
            $nav.removeClass('subnav-fixed')
        }
    }



});

$(".sing-in-btn").bind("click", function (e) {
    e.preventDefault();
    App.SignIn();
    return false;
});

$(".create-account-btn").bind("click", function (e) {
    e.preventDefault();
    App.RegisterAccount();
    return false;
});

$(".send-change-password-link-btn").bind("click", function (e) {
    e.preventDefault();
    App.SendChangePasswordLink();
    return false;
});

$(".change-password-btn").bind("click", function (e) {
    e.preventDefault();
    App.ChangePasswordAccount();
    return false;
});

$(".login-form-field").bind("keypress", function (e) {
    if (e.keyCode == 13) {
        e.preventDefault();
        var allDone = true;
        if ($(this).val() != "") {
            $(".login-form-field").each(function () {
                if ($(this).val() == "") {
                    if ($(this).attr("id") != "sign-member-open-id-name") {
                        allDone = false;
                        $(this).focus();
                        return false;
                    }
                }
            });
        }
        else if ($(this).val() == "") {
            allDone = false;
            return false;
        }

        if (allDone) {
            if ($(".sing-in-btn").length > 0) {
                App.SignIn();
            }
            else if ($(".create-account-btn").length > 0) {
                App.RegisterAccount();
            }
            else if ($(".send-change-password-link-btn").length > 0) {
                App.SendChangePasswordLink();
            }
            else if ($(".change-password-btn").length > 0) {
                App.ChangePasswordAccount();
            }
        }
    }
});

$(function () {
    var tabindex = 1;
    $('input,select').each(function () {
        if (this.type != "hidden") {
            var $input = $(this);
            $input.attr("tabindex", tabindex);
            tabindex++;
        }
    });
});

/*
if ($.browser.webkit) {
$("input").attr('autocomplete', 'off');
}
*/

$(".login-form-field").first().focus();
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function validateNumber(event) {
    var key = window.event ? event.keyCode : event.which;
    if (event.keyCode == 8 || event.keyCode == 46 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 36 || event.keyCode == 35 || key == 43 || key == 44 || key == 45 || key == 46) {
        return true;
    }
    else if (key < 48 || key > 57) {
        return false;
    }
    else return true;
};

$(document).ready(function () {
    $('.profile-field-number').keypress(validateNumber);
});


var provider = "";
var comment = "";
var title = "";
var firstload = true;

$(".btn-open-id").bind("click", function (e) {
    comment = $(this).attr("data-comment");
    $("#open-id-comment").html(comment);

    provider = $(this).attr("data-provider")
    title = $(this).attr("title")

    if (provider == "openid") {
        $("#sign-member-open-id-name").val("http://");
    } else {
        $("#sign-member-open-id-name").val("");
    }

    if (!firstload)
        $("#sign-member-open-id-name").focus();

    firstload = false;
})

$("#sign-member-open-id").bind("click", function (e) {

    if ($("#sign-member-open-id-name").val().length == 0) {
        alert(comment);
        return;
    };

    $("#provider-frendly-name").html(title + " ...");
    $("#redirect-message").css("display", "block");

    window.location.href = "SignInWith?provider=" + provider + "&name=" + encodeURIComponent($("#sign-member-open-id-name").val()) + "&t=" + (new Date()).getTime();
});

$(".social-redirect").bind("click", function (e) {
    $("#provider-frendly-name").html($(this).text() + " ...");
    $("#redirect-message").css("display", "block");
});

$("#signin-livejournla").click();
$("#signin-livejournla").addClass("active");


function calculate_time_zone() {
    var rightNow = new Date();
    var jan1 = new Date(rightNow.getFullYear(), 0, 1, 0, 0, 0, 0);  // jan 1st
    var june1 = new Date(rightNow.getFullYear(), 6, 1, 0, 0, 0, 0); // june 1st
    var temp = jan1.toGMTString();
    var jan2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
    temp = june1.toGMTString();
    var june2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
    var std_time_offset = (jan1 - jan2) / (1000);
    var daylight_time_offset = (june1 - june2) / (1000);
    var dst;
    if (std_time_offset == daylight_time_offset) {
        dst = "0"; // daylight savings time is NOT observed
    } else {
        // positive is southern, negative is northern hemisphere
        var hemisphere = std_time_offset - daylight_time_offset;
        if (hemisphere >= 0)
            std_time_offset = daylight_time_offset;
        dst = "1"; // daylight savings time is observed
    }

    return std_time_offset;
}

// This function is to convert the timezoneoffset to Standard format
function convert(value) {
    var hours = parseInt(value);
    value -= parseInt(value);
    value *= 60;
    var mins = parseInt(value);
    value -= parseInt(value);
    value *= 60;
    var secs = parseInt(value);
    var display_hours = hours;
    // handle GMT case (00:00)
    if (hours == 0) {
        display_hours = "00";
    } else if (hours > 0) {
        // add a plus sign and perhaps an extra 0
        display_hours = (hours < 10) ? "+0" + hours : "+" + hours;
    } else {
        // add an extra 0 if needed
        display_hours = (hours > -10) ? "-0" + Math.abs(hours) : hours;
    }
    mins = (mins < 10) ? "0" + mins : mins;
    return display_hours + ":" + mins;
}

function _addCommas(nStr) {
    var str = Number(nStr);
    var nomChk = parseFloat(str);
    if (nomChk == 0) {
        return 0;
    }

    str += '';
    x = str.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function _removeCommas(nStr) {
    var txt = nStr.replace(/,/g, '');
    return txt;
}

// http://stackoverflow.com/questions/7131909/facebook-callback-appends-to-return-url
if (window.location.hash == '#_=_') {
    window.location.hash = ''; // for older browsers, leaves a # behind
    history.pushState('', document.title, window.location.pathname); // nice and clean
    e.preventDefault(); // no page reload
}
