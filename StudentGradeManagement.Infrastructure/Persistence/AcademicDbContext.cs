
using Microsoft.EntityFrameworkCore;
using StudentGradeManagement.Domain.Entities;

namespace StudentGradeManagement.Infrastructure.Persistence;

public class AcademicDbContext : DbContext
{
    public AcademicDbContext(DbContextOptions<AcademicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Faculty> Faculty => Set<Faculty>();

    public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // STUDENTS
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");

            entity.HasKey(e => e.StudentId);

            entity.Property(e => e.StudentNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.IdentityUserId)
                .HasMaxLength(450);

            entity.HasIndex(e => e.StudentNumber)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.IdentityUserId)
                .IsUnique()
                .HasFilter("[IdentityUserId] IS NOT NULL");
        });

        // FACULTY
        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("Faculty");

            entity.HasKey(e => e.FacultyId);

            entity.Property(e => e.FacultyNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.IdentityUserId)
                .HasMaxLength(450);

            entity.HasIndex(e => e.FacultyNumber)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.IdentityUserId)
                .IsUnique()
                .HasFilter("[IdentityUserId] IS NOT NULL");
        });

        // STUDENT GRADES
        modelBuilder.Entity<StudentGrade>(entity =>
        {
            entity.ToTable("StudentGrades");

            entity.HasKey(e => e.GradeId);

            entity.Property(e => e.SubjectCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Remarks)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Semester)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.SchoolYear)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.DatePosted)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(e => new
            {
                e.StudentId,
                e.SubjectCode,
                e.Semester,
                e.SchoolYear
            }).IsUnique();

            entity.HasOne(e => e.Student)
                .WithMany(e => e.Grades)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Faculty)
                .WithMany(e => e.Grades)
                .HasForeignKey(e => e.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
