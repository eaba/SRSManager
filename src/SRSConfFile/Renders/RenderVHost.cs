using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SrsConfFile.SRSConfClass;

namespace SrsConfFile.Renders
{
    public static class RenderVHost
    {
        private static void Render_rtc(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Rtc == null)
            {
                svcc.Rtc = new Rtc();
            }
            else
            {
                return; //only one
            }

            svcc.Rtc.SectionsName = "rtc";

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Rtc.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "keep_bframe":
                            svcc.Rtc.KeepBFrame = Common.Str2bool(tmpkv.Value);
                            break;
                        case "nack":
                            svcc.Rtc.Nack = Common.Str2bool(tmpkv.Value);
                            break;
                        case "stun_timeout":
                            svcc.Rtc.Stun_timeout = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "stun_strict_check":
                            svcc.Rtc.Stun_strict_check = Common.Str2bool(tmpkv.Value);
                            break;
                        case "nack_no_copy":
                            svcc.Rtc.NackNoCopy = Common.Str2bool(tmpkv.Value);
                            break;
                        case "twcc":
                            svcc.Rtc.Twcc = Common.Str2bool(tmpkv.Value);
                            break;
                        case "dtls_role":
                            svcc.Rtc.DtlsRole = tmpkv.Value;
                            break;
                        case "drop_for_pt":
                            svcc.Rtc.DropForPt = Common.Str2int(tmpkv.Value);
                            break;
                        case "rtmp_to_rtc":
                            svcc.Rtc.RtmpToRtc = Common.Str2bool(tmpkv.Value);
                            break;
                        case "rtc_to_rtmp":
                            svcc.Rtc.RtcToRtmp = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_cluster(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vcluster == null)
            {
                svcc.Vcluster = new Cluster();
            }
            else
            {
                return; //only one
            }

            svcc.Vcluster.SectionsName = "cluster";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "mode":
                            svcc.Vcluster.Mode = tmpkv.Value;
                            break;
                        case "origin":
                            svcc.Vcluster.Origin = tmpkv.Value;
                            break;
                        case "token_traverse":
                            svcc.Vcluster.Token_traverse = Common.Str2bool(tmpkv.Value);
                            break;
                        case "vhost":
                            svcc.Vcluster.Vhost = tmpkv.Value;
                            break;
                        case "origin_cluster":
                            svcc.Vcluster.Origin_cluster = Common.Str2bool(tmpkv.Value);
                            break;
                        case "coworkers":
                            svcc.Vcluster.Coworkers = tmpkv.Value;
                            break;
                        case "debug_srs_upnode":
                            svcc.Vcluster.Debug_srs_upnode = Common.Str2bool(tmpkv.Value);
                            break;
                        case "protocol":
                            svcc.Vcluster.Protocol = (Protocol)Enum.Parse(typeof(Protocol), tmpkv.Value); ;
                            break;
                        case "follow_client":
                            svcc.Vcluster.FollowClient = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_forward(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vforward == null)
            {
                svcc.Vforward = new Forward();
            }
            else
            {
                return; //only one
            }

            svcc.Vforward.SectionsName = "forward";

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vforward.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "destination":
                            svcc.Vforward.Destination = tmpkv.Value;
                            break;
                        case "backend":
                            svcc.Vforward.Backend = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_play(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vplay == null)
            {
                svcc.Vplay = new Play();
            }
            else
            {
                return; //only one
            }

            svcc.Vplay.SectionsName = "play";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "mw_msgs":
                            svcc.Vplay.Mw_msgs = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "gop_cache":
                            svcc.Vplay.Gop_cache = Common.Str2bool(tmpkv.Value);
                            break;
                        case "queue_length":
                            svcc.Vplay.Queue_length = Common.Str2byte(tmpkv.Value);
                            break;
                        case "time_jitter":
                            svcc.Vplay.Time_jitter =
                                (PlayTimeJitter) Enum.Parse(typeof(PlayTimeJitter), tmpkv.Value);
                            break;
                        case "atc":
                            svcc.Vplay.Atc = Common.Str2bool(tmpkv.Value);
                            break;
                        case "mix_correct":
                            svcc.Vplay.Mix_correct = Common.Str2bool(tmpkv.Value);
                            break;
                        case "atc_auto":
                            svcc.Vplay.Atc_auto = Common.Str2bool(tmpkv.Value);
                            break;
                        case "mw_latency":
                            svcc.Vplay.Mw_latency = Common.Str2int(tmpkv.Value);
                            break;
                        case "send_min_interval":
                            svcc.Vplay.Send_min_interval = Common.Str2float(tmpkv.Value);
                            break;
                        case "reduce_sequence_header":
                            svcc.Vplay.Reduce_sequence_header = Common.Str2bool(tmpkv.Value);
                            break;

                        case "gop_cache_max_frames":
                            svcc.Vplay.GopCacheMaxFrames = Common.Str2int(tmpkv.Value);
                            break;
                            
                    }
                }
        }

        private static void Render_publish(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vpublish == null)
            {
                svcc.Vpublish = new Publish();
            }
            else
            {
                return; //only one
            }

            svcc.Vpublish.SectionsName = "publish";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "mr":
                            svcc.Vpublish.Mr = Common.Str2bool(tmpkv.Value);
                            break;
                        case "mr_latency":
                            svcc.Vpublish.Mr_latency = Common.Str2int(tmpkv.Value);
                            break;
                        case "firstpkt_timeout":
                            svcc.Vpublish.Firstpkt_timeout = Common.Str2int(tmpkv.Value);
                            break;
                        case "normal_timeout":
                            svcc.Vpublish.Normal_timeout = Common.Str2int(tmpkv.Value);
                            break;
                        case "parse_sps":
                            svcc.Vpublish.Parse_sps = Common.Str2bool(tmpkv.Value);
                            break;
                        case "try_annexb_first":
                            svcc.Vpublish.Try_Annexb_First = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_refer(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vrefer == null)
            {
                svcc.Vrefer = new Refer();
            }
            else
            {
                return; //only one
            }

            svcc.Vrefer.SectionsName = "refer";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vrefer.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "all":
                            svcc.Vrefer.All = tmpkv.Value;
                            break;
                        case "publish":
                            svcc.Vrefer.Publish = tmpkv.Value;
                            break;
                        case "play":
                            svcc.Vrefer.Play = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_bandcheck(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vbandcheck == null)
            {
                svcc.Vbandcheck = new Bandcheck();
            }
            else
            {
                return; //only one
            }

            svcc.Vbandcheck.SectionsName = "bandcheck";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vbandcheck.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "key":
                            svcc.Vbandcheck.Key = tmpkv.Value;
                            break;
                        case "interval":
                            svcc.Vbandcheck.Interval = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "limit_kbps":
                            svcc.Vbandcheck.Limit_kbps = Common.Str2int(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_security(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vsecurity == null)
            {
                svcc.Vsecurity = new Security();
            }
            else
            {
                return; //only one
            }

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vsecurity.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "allow":
                        case "deny":
                            if (svcc.Vsecurity.Seo == null)
                            {
                                svcc.Vsecurity.Seo = new List<SecurityObj>();
                            }

                            var seo = new SecurityObj();
                            seo.Sem = (SecurityMethod) Enum.Parse(typeof(SecurityMethod), tmpkv.Key);
                            var sArr = Regex.Split(tmpkv.Value, @"[\s]+");
                            // string[] s_arr = tmpkv.Value.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            if (sArr.Length > 1)
                            {
                                seo.Set = (SecurityTarget) Enum.Parse(typeof(SecurityTarget), sArr[0]);
                                seo.Rule = sArr[1].Trim();
                            }

                            svcc.Vsecurity.Seo.Add(seo);
                            break;
                    }
                }
        }

        private static void Render_http_static(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vhttp_static == null)
            {
                svcc.Vhttp_static = new HttpStatic();
            }
            else
            {
                return; //only one
            }

            svcc.Vhttp_static.SectionsName = "http_static";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vhttp_static.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "mount":
                            svcc.Vhttp_static.Mount = tmpkv.Value;
                            break;
                        case "dir":
                            svcc.Vhttp_static.Dir = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_http_remux(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vhttp_remux == null)
            {
                svcc.Vhttp_remux = new HttpRemux();
            }
            else
            {
                return; //only one
            }

            svcc.Vhttp_remux.SectionsName = "http_remux";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vhttp_remux.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "fast_cache":
                            svcc.Vhttp_remux.Fast_cache = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "mount":
                            svcc.Vhttp_remux.Mount = tmpkv.Value;
                            break;
                        case "drop_if_not_match":
                            svcc.Vhttp_remux.Drop_If_Not_Match = Common.Str2bool(tmpkv.Value);
                            break;
                        case "has_audio":
                            svcc.Vhttp_remux.Has_Audio = Common.Str2bool(tmpkv.Value);
                            break;
                        case "has_video":
                            svcc.Vhttp_remux.Has_Video = Common.Str2bool(tmpkv.Value);
                            break;
                        case "guess_has_av":
                            svcc.Vhttp_remux.Guess_Has_Av = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_http_hooks(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vhttp_hooks == null)
            {
                svcc.Vhttp_hooks = new HttpHooks();
            }
            else
            {
                return; //only one
            }

            svcc.Vhttp_hooks.SectionsName = "http_hooks";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vhttp_hooks.Enabled = Common.Str2bool(tmpkv.Value);
                            break;/*
                        case "on_connect":
                            svcc.Vhttp_hooks.On_connect = tmpkv.Value;
                            break;
                        case "on_close":
                            svcc.Vhttp_hooks.On_close = tmpkv.Value;
                            break;*/
                        case "on_publish":
                            svcc.Vhttp_hooks.On_publish = tmpkv.Value;
                            break;
                        case "on_unpublish":
                            svcc.Vhttp_hooks.On_unpublish = tmpkv.Value;
                            break;
                        case "on_play":
                            svcc.Vhttp_hooks.On_play = tmpkv.Value;
                            break;
                        case "on_stop":
                            svcc.Vhttp_hooks.On_stop = tmpkv.Value;
                            break;
                        case "on_dvr":
                            svcc.Vhttp_hooks.On_dvr = tmpkv.Value;
                            break;
                        case "on_hls":
                            svcc.Vhttp_hooks.On_hls = tmpkv.Value;
                            break;
                        case "on_hls_notify":
                            svcc.Vhttp_hooks.On_hls_notify = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_exec(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vexec == null)
            {
                svcc.Vexec = new Exec();
            }
            else
            {
                return; //only one
            }

            svcc.Vexec.SectionsName = "exec";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vexec.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "publish":
                            svcc.Vexec.Publish = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_dash(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vdash == null)
            {
                svcc.Vdash = new Dash();
            }
            else
            {
                return; //only one
            }

            svcc.Vdash.SectionsName = "dash";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vdash.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "dash_fragment":
                            svcc.Vdash.Dash_fragment = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "dash_update_period":
                            svcc.Vdash.Dash_update_period = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "dash_timeshift":
                            svcc.Vdash.Dash_timeshift = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "dash_path":
                            svcc.Vdash.Dash_path = tmpkv.Value;
                            break;
                        case "dash_mpd_file":
                            svcc.Vdash.Dash_mpd_file = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_hls(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vhls == null)
            {
                svcc.Vhls = new Hls();
            }
            else
            {
                return; //only one
            }

            svcc.Vhls.SectionsName = "hls";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vhls.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_fragment":
                            svcc.Vhls.Hls_fragment = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "hls_td_ratio":
                            svcc.Vhls.Hls_td_ratio = Common.Str2float(tmpkv.Value);
                            break;
                        case "hls_aof_ratio":
                            svcc.Vhls.Hls_aof_ratio = Common.Str2float(tmpkv.Value);
                            break;
                        case "hls_window":
                            svcc.Vhls.Hls_window = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "hls_on_error":
                            svcc.Vhls.Hls_on_error = tmpkv.Value;
                            break;
                        case "hls_path":
                            svcc.Vhls.Hls_path = tmpkv.Value;
                            break;
                        case "hls_m3u8_file":
                            svcc.Vhls.Hls_m3u8_file = tmpkv.Value;
                            break;
                        case "hls_ts_file":
                            svcc.Vhls.Hls_ts_file = tmpkv.Value;
                            break;
                        case "hls_ts_floor":
                            svcc.Vhls.Hls_ts_floor = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_entry_prefix":
                            svcc.Vhls.Hls_entry_prefix = tmpkv.Value;
                            break;
                        case "hls_acodec":
                            svcc.Vhls.Hls_acodec = tmpkv.Value;
                            break;
                        case "hls_vcodec":
                            svcc.Vhls.Hls_vcodec = tmpkv.Value;
                            break;
                        case "hls_cleanup":
                            svcc.Vhls.Hls_cleanup = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_dispose":
                            svcc.Vhls.Hls_dispose = Common.Str2int(tmpkv.Value);
                            break;
                        case "hls_nb_notify":
                            svcc.Vhls.Hls_nb_notify = Common.Str2int(tmpkv.Value);
                            break;
                        case "hls_wait_keyframe":
                            svcc.Vhls.Hls_wait_keyframe = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_keys":
                            svcc.Vhls.Hls_keys = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_fragments_per_key":
                            svcc.Vhls.Hls_fragments_per_key = Common.Str2int(tmpkv.Value);
                            break;
                        case "hls_key_file":
                            svcc.Vhls.Hls_key_file = tmpkv.Value;
                            break;
                        case "hls_key_file_path":
                            svcc.Vhls.Hls_key_file_path = tmpkv.Value;
                            break;
                        case "hls_key_url":
                            svcc.Vhls.Hls_key_url = tmpkv.Value;
                            break;
                        case "hls_dts_directly":
                            svcc.Vhls.Hls_dts_directly = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_ctx":
                            svcc.Vhls.Hls_Ctx = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hls_ts_ctx":
                            svcc.Vhls.Hls_Ts_Ctx = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_hds(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vhds == null)
            {
                svcc.Vhds = new Hds();
            }
            else
            {
                return; //only one
            }

            svcc.Vhds.SectionsName = "hds";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vhds.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "hds_fragment":
                            svcc.Vhds.Hds_fragment = Common.Str2int(tmpkv.Value);
                            break;
                        case "hds_window":
                            svcc.Vhds.Hds_window = Common.Str2int(tmpkv.Value);
                            break;
                        case "hds_path":
                            svcc.Vhds.Hds_path = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_dvr(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vdvr == null)
            {
                svcc.Vdvr = new Dvr();
            }
            else
            {
                return; //only one
            }

            svcc.Vdvr.SectionsName = "dvr";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vdvr.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "dvr_apply":
                            svcc.Vdvr.Dvr_apply = tmpkv.Value;
                            break;
                        case "dvr_plan":
                            svcc.Vdvr.Dvr_plan = tmpkv.Value;
                            break;
                        case "dvr_path":
                            svcc.Vdvr.Dvr_path = tmpkv.Value;
                            break;
                        case "dvr_duration":
                            svcc.Vdvr.Dvr_duration = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "dvr_wait_keyframe":
                            svcc.Vdvr.Dvr_wait_keyframe = Common.Str2bool(tmpkv.Value);
                            break;
                        case "time_jitter":
                            svcc.Vdvr.Time_Jitter =
                                (PlayTimeJitter) Enum.Parse(typeof(PlayTimeJitter), tmpkv.Value);
                            break;
                    }
                }
        }


        private static void Render_nack(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vnack == null)
            {
                svcc.Vnack = new Nack();
            }
            else
            {
                return; //only one
            }

            svcc.Vnack.SectionsName = "nack";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Vnack.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Render_ingest_input(SectionBody scbin, Ingest igs, string instanceName = "")
        {
            if (igs.Input == null)
            {
                igs.Input = new IngestInput();
            }
            else
            {
                return; //only one
            }


            igs.Input.SectionsName = "input";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "type":
                            igs.Input.Type = (IngestInputType) Enum.Parse(typeof(IngestInputType), tmpkv.Value);
                            break;
                        case "url":
                            igs.Input.Url = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Render_ingest_engine(SectionBody scbin, Ingest igs, string instanceName = "")
        {
            if (igs.Engines == null) igs.Engines = new List<IngestTranscodeEngine>();
            if (null != igs.Engines .Find(s => s.InstanceName == instanceName))
                return; //filter the same engiest instance
            var eng = new IngestTranscodeEngine();
            eng.EngineName = instanceName;
            eng.InstanceName = instanceName;
            eng.SectionsName = "engine";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            eng.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "vcodec":
                            eng.Vcodec = tmpkv.Value;
                            break;
                        case "vbitrate":
                            eng.Vbitrate = Common.Str2int(tmpkv.Value);
                            break;
                        case "vfps":
                            eng.Vfps = Common.Str2int(tmpkv.Value);
                            break;
                        case "vwidth":
                            eng.Vwidth = Common.Str2int(tmpkv.Value);
                            break;
                        case "vheight":
                            eng.Vheight = Common.Str2int(tmpkv.Value);
                            break;
                        case "vthreads":
                            eng.Vthreads = Common.Str2int(tmpkv.Value);
                            break;
                        case "acodec":
                            eng.Acodec = tmpkv.Value;
                            break;
                        case "abitrate":
                            eng.Abitrate = Common.Str2int(tmpkv.Value);
                            break;
                        case "asample_rate":
                            eng.Asample_rate = Common.Str2int(tmpkv.Value);
                            break;
                        case "achannels":
                            eng.Achannels = Common.Str2int(tmpkv.Value);
                            break;
                        case "output":
                            eng.Output = tmpkv.Value;
                            break;
                        case "iformat":
                            eng.Iformat =
                                (IngestEngineIoformat) Enum.Parse(typeof(IngestEngineIoformat), tmpkv.Value);
                            break;
                        case "oformat":
                            eng.Oformat =
                                (IngestEngineIoformat) Enum.Parse(typeof(IngestEngineIoformat), tmpkv.Value);
                            break;
                        case "vprofile":
                            eng.Vprofile =
                                (IngestEngineVprofile) Enum.Parse(typeof(IngestEngineVprofile), tmpkv.Value);
                            break;
                        case "vpreset":
                            eng.Vpreset =
                                (IngestEngineVpreset) Enum.Parse(typeof(IngestEngineVpreset), tmpkv.Value);
                            break;
                    }
                }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "perfile":
                            Rander_ingest_engine_perfile(scb, eng, tmpkv.Value);
                            break;

                        case "vfilter":
                            Rander_ingest_engine_vfilter(scb, eng, tmpkv.Value);
                            break;


                        case "vparams":
                            Rander_ingest_engine_vparams(scb, eng, tmpkv.Value);
                            break;
                        case "aparams":
                            Rander_ingest_engine_aparams(scb, eng, tmpkv.Value);
                            break;
                    }
                }

            igs.Engines.Add(eng);
        }

        private static void Render_transcode_engine(SectionBody scbin, Transcode tsc, string instanceName = "")
        {
            if (tsc.Engines == null) tsc.Engines = new List<IngestTranscodeEngine>();
            if (null != tsc.Engines .Find(s => s.InstanceName == instanceName))
                return; //filter the same engiest instance
            var eng = new IngestTranscodeEngine
            {
                EngineName = instanceName, InstanceName = instanceName, SectionsName = "engine"
            };
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            eng.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "vcodec":
                            eng.Vcodec = tmpkv.Value;
                            break;
                        case "vbitrate":
                            eng.Vbitrate = Common.Str2int(tmpkv.Value);
                            break;
                        case "vfps":
                            eng.Vfps = Common.Str2int(tmpkv.Value);
                            break;
                        case "vwidth":
                            eng.Vwidth = Common.Str2int(tmpkv.Value);
                            break;
                        case "vheight":
                            eng.Vheight = Common.Str2int(tmpkv.Value);
                            break;
                        case "vthreads":
                            eng.Vthreads = Common.Str2int(tmpkv.Value);
                            break;
                        case "acodec":
                            eng.Acodec = tmpkv.Value;
                            break;
                        case "abitrate":
                            eng.Abitrate = Common.Str2int(tmpkv.Value);
                            break;
                        case "asample_rate":
                            eng.Asample_rate = Common.Str2int(tmpkv.Value);
                            break;
                        case "achannels":
                            eng.Achannels = Common.Str2int(tmpkv.Value);
                            break;
                        case "output":
                            eng.Output = tmpkv.Value;
                            break;
                        case "iformat":
                            eng.Iformat =
                                (IngestEngineIoformat) Enum.Parse(typeof(IngestEngineIoformat), tmpkv.Value);
                            break;
                        case "oformat":
                            eng.Oformat =
                                (IngestEngineIoformat) Enum.Parse(typeof(IngestEngineIoformat), tmpkv.Value);
                            break;
                        case "vprofile":
                            eng.Vprofile =
                                (IngestEngineVprofile) Enum.Parse(typeof(IngestEngineVprofile), tmpkv.Value);
                            break;
                        case "vpreset":
                            eng.Vpreset =
                                (IngestEngineVpreset) Enum.Parse(typeof(IngestEngineVpreset), tmpkv.Value);
                            break;
                    }
                }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "perfile":
                            Rander_ingest_engine_perfile(scb, eng, tmpkv.Value);
                            break;

                        case "vfilter":
                            Rander_ingest_engine_vfilter(scb, eng, tmpkv.Value);
                            break;
                        case "vparams":
                            Rander_ingest_engine_vparams(scb, eng, tmpkv.Value);
                            break;
                        case "aparams":
                            Rander_ingest_engine_aparams(scb, eng, tmpkv.Value);
                            break;
                    }
                }

            tsc.Engines.Add(eng);
        }

        private static void Rander_ingest_engine_perfile(SectionBody scbin, IngestTranscodeEngine iten,
            string instanceName = "")
        {
            if (iten.Perfile == null)
            {
                iten.Perfile = new IngestEnginePerfile();
            }
            else
            {
                return; //only one
            }

            iten.Perfile.SectionsName = "perfile";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key) && s.Trim() != "re;")
                    {
                        continue;
                    }
                    else if (s.Trim() == "re;")
                    {
                        tmpkv = new KeyValuePair<string, string>("re", "re"); //特殊参数处理
                    }

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "re":
                            iten.Perfile.Re = tmpkv.Value;
                            break;
                        case "rtsp_transport":
                            iten.Perfile.Rtsp_transport = tmpkv.Value;
                            break;
                    }
                }
        }

        private static void Rander_ingest_engine_vfilter(SectionBody scbin, IngestTranscodeEngine iten,
            string instanceName = "")
        {
            if (iten.Vfilter == null)
            {
                iten.Vfilter = new IngestEngineVfilter();
            }
            else
            {
                return; //only one
            }

            iten.Vfilter.SectionsName = "vfilter";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "i":
                            iten.Vfilter.I = tmpkv.Value;
                            break;
                        case "vf":
                            iten.Vfilter.Vf = tmpkv.Value;
                            break;
                        case "filter_complex":
                            iten.Vfilter.Filter_Complex = tmpkv.Value;
                            break;
                    }
                }
        }


        private static void Rander_ingest_engine_vparams(SectionBody scbin, IngestTranscodeEngine iten,
            string instanceName = "")
        {
            if (iten.Vparams == null)
            {
                iten.Vparams = new IngestEngineVparams();
            }
            else
            {
                return; //only one
            }

            iten.Vparams.SectionsName = "vparams";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "t":
                            iten.Vparams.T = Common.Str2int(tmpkv.Value);
                            break;
                        case "coder":
                            iten.Vparams.Coder = Common.Str2int(tmpkv.Value);
                            break;
                        case "b_strategy":
                            iten.Vparams.B_strategy = Common.Str2int(tmpkv.Value);
                            break;
                        case "bf":
                            iten.Vparams.Bf = Common.Str2int(tmpkv.Value);
                            break;
                        case "refs":
                            iten.Vparams.Refs = Common.Str2int(tmpkv.Value);
                            break;
                    }
                }
        }

        private static void Rander_ingest_engine_aparams(SectionBody scbin, IngestTranscodeEngine iten,
            string instanceName = "")
        {
            if (iten.Aparams == null)
            {
                iten.Aparams = new IngestEngineAparams();
            }
            else
            {
                return; //only one
            }

            iten.Aparams.SectionsName = "aparams";
            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "profile:a":
                            iten.Aparams.Profile_a = tmpkv.Value;
                            break;
                        case "bsf:a":
                            iten.Aparams.Bsf_a = tmpkv.Value;
                            break;
                    }
                }
        }


        private static void Render_ingest(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vingests == null) svcc.Vingests = new List<Ingest>();
            if (null != svcc.Vingests .Find(s => s.InstanceName == instanceName))
                return; //filter the same ingest instance
            var igs = new Ingest();

            igs.IngestName = instanceName;
            igs.InstanceName = instanceName;
            igs.SectionsName = "ingest";

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            igs.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "ffmpeg":
                            igs.Ffmpeg = tmpkv.Value;
                            break;
                    }
                }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "input":
                            Render_ingest_input(scb, igs, tmpkv.Value);
                            break;
                        case "engine":
                            Render_ingest_engine(scb, igs, tmpkv.Value);
                            break;
                    }
                }

            svcc.Vingests.Add(igs);
        }

        private static void Render_transcode(SectionBody scbin, SrsvHostConfClass svcc, string instanceName = "")
        {
            if (svcc.Vtranscodes == null) svcc.Vtranscodes = new List<Transcode>();
            if (null != svcc.Vtranscodes .Find(s => s.InstanceName == instanceName))
                return; //filter the same ingest instance
            var trc = new Transcode();

            trc.InstanceName = instanceName;
            trc.SectionsName = "transcode";

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;
                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            trc.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "ffmpeg":
                            trc.Ffmpeg = tmpkv.Value;
                            break;
                    }
                }

            if (scbin.SubSections != null)
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "engine":
                            Render_transcode_engine(scb, trc, tmpkv.Value);
                            break;
                    }
                }

            svcc.Vtranscodes.Add(trc);
        }

        public static void Render(SectionBody scbin, SrsSystemConfClass sccout, string instanceName = "")
        {
            if (sccout.Vhosts == null) sccout.Vhosts = new List<SrsvHostConfClass>();

            if (null != sccout.Vhosts.Find(s => s.InstanceName == instanceName))
                return; //filter the same vhost instance

            var svcc = new SrsvHostConfClass();
            svcc.InstanceName = instanceName;
            svcc.SectionsName = "vhost";
            svcc.VhostDomain = svcc.InstanceName;

            if (scbin.BodyList != null)
                foreach (var s in scbin.BodyList)
                {
                    if (!s.Trim().EndsWith(";")) continue;
                    var tmpkv = Common.GetKV(s);
                    if (string.IsNullOrEmpty(tmpkv.Key)) continue;

                    var cmd = tmpkv.Key.Trim().ToLower();
                    switch (cmd)
                    {
                        case "enabled":
                            svcc.Enabled = Common.Str2bool(tmpkv.Value);
                            break;
                        case "min_latency":
                            svcc.Min_latency = Common.Str2bool(tmpkv.Value);
                            break;
                        case "tcp_nodelay":
                            svcc.Tcp_nodelay = Common.Str2bool(tmpkv.Value);
                            break;
                        case "chunk_size":
                            svcc.Chunk_size = Common.Str2ushort(tmpkv.Value);
                            break;
                        case "in_ack_size":
                            svcc.In_ack_size = Common.Str2int(tmpkv.Value);
                            break;
                        case "out_ack_size":
                            svcc.Out_ack_size = Common.Str2int(tmpkv.Value);
                            break;
                    }
                }

            if (scbin.SubSections != null && scbin.SubSections.Count > 0)
            {
                foreach (var scb in scbin.SubSections)
                {
                    var tmpkv = Common.GetSectionNameAndInstanceNameValue(scb);
                    var cmd = tmpkv.Key;
                    switch (cmd)
                    {
                        case "rtc":
                            Render_rtc(scb, svcc, tmpkv.Value);
                            break;
                        case "cluster":
                            Render_cluster(scb, svcc, tmpkv.Value);
                            break;
                        case "forward":
                            Render_forward(scb, svcc, tmpkv.Value);
                            break;
                        case "play":
                            Render_play(scb, svcc, tmpkv.Value);
                            break;
                        case "publish":
                            Render_publish(scb, svcc, tmpkv.Value);
                            break;
                        case "refer":
                            Render_refer(scb, svcc, tmpkv.Value);
                            break;
                        case "bandcheck":
                            Render_bandcheck(scb, svcc, tmpkv.Value);
                            break;
                        case "security":
                            Render_security(scb, svcc, tmpkv.Value);
                            break;
                        case "http_static":
                            Render_http_static(scb, svcc, tmpkv.Value);
                            break;
                        case "http_remux":
                            Render_http_remux(scb, svcc, tmpkv.Value);
                            break;
                        case "http_hooks":
                            Render_http_hooks(scb, svcc, tmpkv.Value);
                            break;
                        case "exec":
                            Render_exec(scb, svcc, tmpkv.Value);
                            break;
                        case "dash":
                            Render_dash(scb, svcc, tmpkv.Value);
                            break;
                        case "hls":
                            Render_hls(scb, svcc, tmpkv.Value);
                            break;
                        case "hds":
                            Render_hds(scb, svcc, tmpkv.Value);
                            break;
                        case "dvr":
                            Render_dvr(scb, svcc, tmpkv.Value);
                            break;
                        case "ingest":
                            Render_ingest(scb, svcc, tmpkv.Value);
                            break;
                        case "transcode":
                            Render_transcode(scb, svcc, tmpkv.Value);
                            break;
                        case "nack":
                            Render_nack(scb, svcc, tmpkv.Value);
                            break;
                    }
                }
            }

            sccout.Vhosts.Add(svcc);
        }
    }
}