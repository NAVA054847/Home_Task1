using Microsoft.EntityFrameworkCore;
using RefundSystem.Core.Entities;
using RefundSystem.Core.ReadModels;

namespace RefundSystem.Data.Context;


/// הקשר למסד הנתונים – מייצג את כל הטבלאות והישויות במערכת ההחזרים.
/// משמש את ה-Repositories לגישה לנתונים.

public partial class RefundSystemDbContext : DbContext
{
    /// Constructor ריק – נדרש ל-EF Migrations (למשל: dotnet ef migrations add).
    public RefundSystemDbContext()
    {
    }

    /// Constructor עם options – נדרש בזמן ריצה. ה-options מגיעים מ-Program.cs (connection string וכו').
    public RefundSystemDbContext(DbContextOptions<RefundSystemDbContext> options)
        : base(options)
    {
    }

    /// טבלת תקציבים – תקציב חודשי לפי שנה וחודש.
    public virtual DbSet<Budget> Budgets { get; set; }

    /// טבלת אזרחים.
    public virtual DbSet<Citizen> Citizens { get; set; }

    /// טבלת פקידים.
    public virtual DbSet<Clerk> Clerks { get; set; }

    /// טבלת הכנסות חודשיות – הכנסה לפי אזרח, שנת מס וחודש.
    public virtual DbSet<MonthlyIncome> MonthlyIncomes { get; set; }

    /// טבלת בקשות החזר.
    public virtual DbSet<RefundRequest> RefundRequests { get; set; }

    /// טבלת סטטוסים 
    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }


    /// הגדרת המיפוי בין ה-Entities לטבלאות – מפתחות, אינדקסים, קשרים ואילוצים.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Read Model מפרוצדורה – אין טבלה, אין מפתח. משמש ל-FromSqlRaw בלבד.
        modelBuilder.Entity<PendingRequestReadModel>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });


        // פקיד – מפתח ראשי, ת.ז. ייחודית
        modelBuilder.Entity<Clerk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clerks__3214EC07D29DFD59");

            entity.Property(e => e.IdentityNumber).HasMaxLength(9);
            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasIndex(e => e.IdentityNumber, "IX_Clerks_IdentityNumber").IsUnique();
        });

        // תקציב – שנה+חודש ייחודיים, RowVersion ל-Optimistic Concurrency
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Budgets__3214EC076E605BA9");

            entity.HasIndex(e => new { e.Year, e.Month }, "UQ_Budget").IsUnique();

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TotalBudget).HasColumnType("decimal(14, 2)");
            entity.Property(e => e.UsedBudget).HasColumnType("decimal(14, 2)");
        });

        // אזרח – מפתח ראשי, ת.ז. ייחודית
        modelBuilder.Entity<Citizen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Citizens__3214EC0759D72142");

            entity.HasIndex(e => e.IdentityNumber, "UQ__Citizens__6354A73F6DD81155").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IdentityNumber).HasMaxLength(20);
        });

        // הכנסה חודשית – אזרח+שנת מס+חודש ייחודיים, קשר לאזרח
        modelBuilder.Entity<MonthlyIncome>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MonthlyI__3214EC0794180316");

            entity.HasIndex(e => new { e.CitizenId, e.TaxYear, e.Month }, "UQ_MonthlyIncome").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Citizen).WithMany(p => p.MonthlyIncomes)
                .HasForeignKey(d => d.CitizenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonthlyIncomes_Citizens");
        });

        // בקשת החזר – אזרח+שנת מס ייחודיים, קשרים לאזרח ולסטטוס, CreatedAt ברירת מחדל
        modelBuilder.Entity<RefundRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefundRe__3214EC074AC54EF6");

            entity.HasIndex(e => new { e.CitizenId, e.TaxYear }, "UQ_RefundRequests").IsUnique();

            entity.Property(e => e.ApprovedAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CalculatedAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Citizen).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.CitizenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefundRequests_Citizens");

            entity.HasOne(d => d.Status).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefundRequests_Status");
        });

        // סטטוס בקשה 
        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestS__3214EC07FC1C8A09");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    /// נקודת הרחבה – מאפשר הוספת הגדרות בקובץ partial נפרד.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
