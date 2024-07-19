using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartClinics.Models;

namespace SmartClinics.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.NationalID);

            modelBuilder.Entity<Patient>()
                .HasKey(d => d.NationalID);

            modelBuilder.Entity<DoctorRequestsToJoin>()
                .HasKey(d => d.NationalID);
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<MedicalInvestigate> MedicalInvestigates { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DoctorRating> DoctorRatings { get; set; }
        public DbSet<DoctorRequestsToJoin> doctorRequestsToJoins { get; set; }
    }
}
