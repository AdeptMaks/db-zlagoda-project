using System.Data;
using FluentMigrator;
using FluentMigrator.Runner.BatchParser;

namespace Api.Data.Migrations;

[Migration(202606142248)]
public class Init : Migration
{
    public override void Up()
    {
        Create.Table("employee")
            .WithColumn("id_employee").AsString(10).PrimaryKey()
            .WithColumn("empl_surname").AsString(50).NotNullable()
            .WithColumn("empl_name").AsString(50).NotNullable()
            .WithColumn("empl_patronymic").AsString(50).Nullable()
            .WithColumn("empl_role").AsString(10).NotNullable()
            .WithColumn("salary").AsDecimal(13, 4).NotNullable()
            .WithColumn("date_of_birth").AsDate().NotNullable()
            .WithColumn("date_of_start").AsDate().NotNullable()
            .WithColumn("phone_number").AsString(13).NotNullable()
            .WithColumn("city").AsString(50).NotNullable()
            .WithColumn("street").AsString(50).NotNullable()
            .WithColumn("zip_code").AsString(9).NotNullable();

        Create.Table("category")
            .WithColumn("category_number").AsInt32().PrimaryKey()
            .WithColumn("category_name").AsString(50).NotNullable();

        Create.Table("customer_card")
            .WithColumn("card_number").AsString(13).PrimaryKey()
            .WithColumn("cust_surname").AsString(50).NotNullable()
            .WithColumn("cust_name").AsString(50).NotNullable()
            .WithColumn("cust_patronymic").AsString(50).Nullable()
            .WithColumn("phone_number").AsString(13).NotNullable()
            .WithColumn("city").AsString(50).Nullable()
            .WithColumn("street").AsString(50).Nullable()
            .WithColumn("zip_code").AsString(9).Nullable()
            .WithColumn("percent").AsInt32().NotNullable();

        Create.Table("product")
            .WithColumn("id_product").AsInt32().PrimaryKey()
            .WithColumn("category_number").AsInt32().NotNullable()
            .WithColumn("product_name").AsString(50).NotNullable()
            .WithColumn("characteristics").AsString(100).NotNullable();

        Create.ForeignKey("fk_product_category")
            .FromTable("product").ForeignColumn("category_number")
            .ToTable("category").PrimaryColumn("category_number")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.None);

        Create.Table("store_product")
            .WithColumn("UPC").AsString(12).PrimaryKey()
            .WithColumn("UPC_prom").AsString(12).Nullable()
            .WithColumn("id_product").AsInt32().NotNullable()
            .WithColumn("selling_price").AsDecimal(13, 4).NotNullable()
            .WithColumn("products_number").AsInt32().NotNullable()
            .WithColumn("promotional_product").AsBoolean().NotNullable();

        Create.ForeignKey("fk_store_product_prom")
            .FromTable("store_product").ForeignColumn("UPC_prom")
            .ToTable("store_product").PrimaryColumn("UPC")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.SetNull);

        Create.ForeignKey("fk_store_product_product")
            .FromTable("store_product").ForeignColumn("id_product")
            .ToTable("product").PrimaryColumn("id_product")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.None);

        Create.Table("store_check")
            .WithColumn("check_number").AsString(10).PrimaryKey()
            .WithColumn("id_employee").AsString(10).NotNullable()
            .WithColumn("card_number").AsString(13).Nullable()
            .WithColumn("print_date").AsDateTime().NotNullable()
            .WithColumn("sum_total").AsDecimal(13, 4).NotNullable()
            .WithColumn("vat").AsDecimal(13, 4).NotNullable();

        Create.ForeignKey("fk_check_employee")
            .FromTable("store_check").ForeignColumn("id_employee")
            .ToTable("employee").PrimaryColumn("id_employee")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.None);

        Create.ForeignKey("fk_check_card")
            .FromTable("store_check").ForeignColumn("card_number")
            .ToTable("customer_card").PrimaryColumn("card_number")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.None);

        Create.Table("sale")
            .WithColumn("UPC").AsString(12).NotNullable()
            .WithColumn("check_number").AsString(10).NotNullable()
            .WithColumn("product_number").AsInt32().NotNullable()
            .WithColumn("selling_price").AsDecimal(13, 4).NotNullable();

        Create.PrimaryKey("pk_sale").OnTable("sale").Columns("UPC", "check_number");

        Create.ForeignKey("fk_sale_store_product")
            .FromTable("sale").ForeignColumn("UPC")
            .ToTable("store_product").PrimaryColumn("UPC")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.None);

        Create.ForeignKey("fk_sale_check")
            .FromTable("sale").ForeignColumn("check_number")
            .ToTable("store_check").PrimaryColumn("check_number")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("sale");
        Delete.Table("store_check");
        Delete.Table("store_product");
        Delete.Table("product");
        Delete.Table("customer_card");
        Delete.Table("category");
        Delete.Table("employee");
    }
}
