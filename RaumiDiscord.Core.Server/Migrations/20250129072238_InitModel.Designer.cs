﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RaumiDiscord.Core.Server.DataContext;

#nullable disable

namespace RaumiDiscord.Core.Server.Migrations
{
    [DbContext(typeof(DeltaRaumiDbContext))]
    [Migration("20250129072238_InitModel")]
    partial class InitModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.DiscordComponentModel", b =>
                {
                    b.Property<Guid>("CustomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeltaRaumiComponentType")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CustomId");

                    b.ToTable("Components");
                });

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.GuildBaseData", b =>
                {
                    b.Property<ulong>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BannerUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("GuildName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IconUrl")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("MaxUploadLimit")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberCount")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("OwnerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PremiumSubscriptionCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PremiumTier")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("WelcomeChannnelID")
                        .HasColumnType("INTEGER");

                    b.HasKey("GuildId");

                    b.ToTable("GuildBases");
                });

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.UrlDetaModel", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TTL")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("UrlType")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UrlDataModels");
                });

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.UserBaseData", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Barthday")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsWebhook")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Isbot")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("UserBases");
                });

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.UserGuildData", b =>
                {
                    b.Property<string>("GuildAvatarId")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuildLebel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuildUserFlags")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimedOutUntil")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalMessage")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("GuildId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGuildData");
                });

            modelBuilder.Entity("RaumiDiscord.Core.Server.Api.Models.UserGuildData", b =>
                {
                    b.HasOne("RaumiDiscord.Core.Server.Api.Models.GuildBaseData", "GuildBaseData")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RaumiDiscord.Core.Server.Api.Models.UserBaseData", "UserBaseData")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildBaseData");

                    b.Navigation("UserBaseData");
                });
#pragma warning restore 612, 618
        }
    }
}
