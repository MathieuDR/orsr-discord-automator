﻿using System;
using Discord;
using Discord.Commands;

namespace DiscordBotFanatic.Helpers {
    public static class EmbedHelper {
        public static EmbedFooterBuilder CreateFooterFromMessageAuthor(this SocketCommandContext context,
            string appendToFooter = null) {
            string footerText = $"Requested by {context.User.Username}.";
            if (!string.IsNullOrWhiteSpace(appendToFooter)) {
                footerText += $", {appendToFooter}";
            }

            return new EmbedFooterBuilder() {
                IconUrl = context.User.GetAvatarUrl(),
                Text = footerText
            };
        }

        public static EmbedAuthorBuilder CreateAuthorFromMessageAuthor(this SocketCommandContext context, string url = null,
            string appendToFooter = null) {
            string footerText = $"{context.User.Username}";
            if (!string.IsNullOrWhiteSpace(appendToFooter)) {
                footerText += $", {appendToFooter}";
            }

            return new EmbedAuthorBuilder() {
                IconUrl = context.User.GetAvatarUrl(),
                Name = footerText,
                Url = url
            };
        }

        public static EmbedAuthorBuilder CreateWiseOldManAuthorBuilder(string resourceName = null, string resourceUrl = null) {
            return new EmbedAuthorBuilder() {
                Name = string.IsNullOrWhiteSpace(resourceName) ? "Wiseoldman.net" : $"Wiseoldman.net - {resourceName}",
                IconUrl = "https://wiseoldman.net/img/logo.png",
                Url = string.IsNullOrWhiteSpace(resourceUrl) ? "" : resourceUrl
            };
        }

        public static EmbedBuilder CreateCommonWiseOldManEmbedBuilder(this SocketCommandContext context,
            string appendToFooter = null, string resourceUrl = null, string resource = null) {
            var builder = CreateCommonEmbedBuilder(context);
            builder.AddWiseOldManInfo(context, appendToFooter, resourceUrl, resource);
            return builder;
        }

        public static EmbedBuilder CreateCommonEmbedBuilder(this SocketCommandContext context) {
            return CreateCommonEmbedBuilder();
        }

        public static EmbedBuilder CreateCommonEmbedBuilder(this ICommandContext context) {
            return CreateCommonEmbedBuilder();
        }

        public static EmbedBuilder CreateCommonEmbedBuilder() {
            return new EmbedBuilder() {
                Timestamp = DateTimeOffset.Now,
                Color = Color.DarkPurple
            };
        }



        public static EmbedBuilder AddWiseOldManInfo(this EmbedBuilder builder, SocketCommandContext context,
            string appendToFooter = null,
            string resourceUrl = null, string resource = null) {
            builder.Footer = context.CreateFooterFromMessageAuthor(appendToFooter);
            builder.Author = CreateWiseOldManAuthorBuilder(resource, resourceUrl);
            return builder;
        }

        public static EmbedBuilder AddWiseOldManAuthor(this EmbedBuilder builder, string resourceName = null,
            string resourceUrl = null) {
            builder.Author = CreateWiseOldManAuthorBuilder(resourceName, resourceUrl);
            return builder;
        }

        public static EmbedBuilder AddFooterFromMessageAuthor(this EmbedBuilder builder, SocketCommandContext context,
            string appendToFooter = null) {

            builder.Footer = context.CreateFooterFromMessageAuthor(appendToFooter);
            return builder;
        }

        public static EmbedBuilder AddAuthorFromMessageAuthor(this EmbedBuilder builder, SocketCommandContext context,
            string url = null, string appendToFooter = null) {
            builder.Author = context.CreateAuthorFromMessageAuthor(url, appendToFooter);
            return builder;
        }
    }
}