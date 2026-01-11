using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Areas.Identity.Data;
using TaskManager.Data;
using TaskManager.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            if (context.Roles.Any())
                return;
            context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "e93d014f-bd80-458b-bcb9-c29fdaf7cba6", Name = "Admin", NormalizedName = "Admin".ToUpper() 
                },

                new IdentityRole
                {
                    Id = "4aa12e41-b16f-432a-8166-62fe0543d1ef", Name = "Member", NormalizedName = "Member".ToUpper() }

                );

                var hasher = new PasswordHasher<ApplicationUser>();

            context.Users.AddRange(
                new ApplicationUser

                {

                    Id = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(new ApplicationUser(),"Admin1!")
                },
                new ApplicationUser

                {

                    Id = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1",
                    UserName = "member@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "MEMBER@TEST.COM",
                    Email = "member@test.com",
                    NormalizedUserName = "MEMBER@TEST.COM",
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Member1!")
                },

                new ApplicationUser

                {

                    Id = "d3f5e8c1-3b6e-4f4a-9f4a-1c2e3d4f5a6b",
                    UserName = "tester@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "TESTER@TEST.COM",
                    Email = "tester@test.com",
                    NormalizedUserName = "TESTER@TEST.COM",
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Tester1!")
                }
            );
            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {

                    RoleId = "e93d014f-bd80-458b-bcb9-c29fdaf7cba6",
                    UserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8"
                },

                new IdentityUserRole<string>

                {

                    RoleId = "4aa12e41-b16f-432a-8166-62fe0543d1ef",
                    UserId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1"
                }
            );

            // Seed Project
            context.Projects.AddRange(
                new Project
                {
                    Title = "Proiect Inițial",
                    Description = "Acesta este proiectul de seed pentru task-urile inițiale.",
                    CreatorId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                    DateCreated = DateTime.Now
                },
                new Project
                {
                    Title = "Platformă Web Admin",
                    Description = "Dezvoltare și mentenanță platformă web pentru admin.",
                    CreatorId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                    DateCreated = DateTime.Now.AddDays(-5)
                },
                new Project
                {
                    Title = "Aplicație Mobile",
                    Description = "Dezvoltare aplicație mobilă cross-platform pentru task management.",
                    CreatorId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1",
                    DateCreated = DateTime.Now.AddDays(-3)
                }
            );
            context.SaveChanges();

            var seedProject = context.Projects
                .Include(p => p.Members)
                .FirstOrDefault(p => p.Title == "Proiect Inițial");
            if (seedProject == null) return;

            var memberUser = context.Users.FirstOrDefault(u => u.Id == "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1");
            if (memberUser != null && !seedProject.Members.Any(m => m.Id == memberUser.Id))
            {
                seedProject.Members.Add(memberUser);
                context.SaveChanges();
            }

            context.ProjectTasks.AddRange(
                new ProjectTask
                {
                    Title = "Initial Task 1",
                    Description = "This is the first seeded task.",
                    Status = TaskManager.Models.TaskStatus.NotStarted,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(5),
                    AssignedUserId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1", // member@test.com
                    ProjectId = seedProject.Id
                },
                new ProjectTask
                {
                    Title = "Initial Task 2",
                    Description = "This is the second seeded task.",
                    Status = TaskManager.Models.TaskStatus.InProgress,
                    StartDate = DateTime.Now.AddDays(-2),
                    EndDate = DateTime.Now.AddDays(3),
                    MediaUrl = "https://www.youtube.com/embed/PErrvYtVzbk",
                    AssignedUserId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1",
                    ProjectId = seedProject.Id
                },
                new ProjectTask
                {
                    Title = "Cat in a fish bowl",
                    Description = "A funny video of a cat stuck in a fish bowl.",
                    Status = TaskManager.Models.TaskStatus.Completed,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(-5),
                    MediaUrl = "https://www.youtube.com/embed/9FjGP4t2zKY",
                    AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                    ProjectId = seedProject.Id
                }
            );
                            
            context.SaveChanges();

            var adminProject = context.Projects.FirstOrDefault(p => p.Title == "Platformă Web Admin");
            if (adminProject != null)
            {
                context.ProjectTasks.AddRange(
                    new ProjectTask
                    {
                        Title = "Design sistem autentificare",
                        Description = "Implementare sistem de login cu ASP.NET Identity.",
                        Status = TaskManager.Models.TaskStatus.Completed,
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now.AddDays(-2),
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = adminProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Configurare baze de date",
                        Description = "Setare și optimizare baze de date pentru aplicație.",
                        Status = TaskManager.Models.TaskStatus.InProgress,
                        StartDate = DateTime.Now.AddDays(-3),
                        EndDate = DateTime.Now.AddDays(4),
                        MediaUrl = "/images/db.jpg",
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = adminProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Testare funcționalități",
                        Description = "Testare completă a tuturor funcționalităților aplicației.",
                        Status = TaskManager.Models.TaskStatus.NotStarted,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(7),
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = adminProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Optimizare performanță",
                        Description = "Îmbunătățirea vitezei și performanței aplicației web.",
                        Status = TaskManager.Models.TaskStatus.NotStarted,
                        StartDate = DateTime.Now.AddDays(1),
                        EndDate = DateTime.Now.AddDays(10),
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = adminProject.Id
                    }
                );
                context.SaveChanges();
            }

            var mobileProject = context.Projects
                .Include(p => p.Members)
                .FirstOrDefault(p => p.Title == "Aplicație Mobile");
            if (mobileProject != null)
            {
                var adminProjectMember = context.Users.FirstOrDefault(u => u.Id == "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8");
                if (adminProjectMember != null && !mobileProject.Members.Any(m => m.Id == adminProjectMember.Id))
                {
                    mobileProject.Members.Add(adminProjectMember);
                    context.SaveChanges();
                }

                context.ProjectTasks.AddRange(
                    new ProjectTask
                    {
                        Title = "Design UI/UX",
                        Description = "Creare design modern și intuitiv pentru aplicația mobilă.",
                        Status = TaskManager.Models.TaskStatus.Completed,
                        StartDate = DateTime.Now.AddDays(-7),
                        EndDate = DateTime.Now.AddDays(-3),
                        AssignedUserId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1",
                        ProjectId = mobileProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Implementare funcționalități",
                        Description = "Dezvoltare funcționalități core: autentificare, liste task-uri, notificări.",
                        Status = TaskManager.Models.TaskStatus.InProgress,
                        StartDate = DateTime.Now.AddDays(-2),
                        EndDate = DateTime.Now.AddDays(5),
                        MediaUrl = "https://www.youtube.com/embed/WBF_ZmjdZ1I",
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = mobileProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Testing și debugging",
                        Description = "Testare aplicație pe multiple dispozitive și rezolvare bug-uri.",
                        Status = TaskManager.Models.TaskStatus.NotStarted,
                        StartDate = DateTime.Now.AddDays(3),
                        EndDate = DateTime.Now.AddDays(8),
                        AssignedUserId = "5b54ca8a-aba8-49bd-9a6e-503ae66fd5d1",
                        ProjectId = mobileProject.Id
                    },
                    new ProjectTask
                    {
                        Title = "Deploy pe store-uri",
                        Description = "Publicare aplicație pe Google Play și App Store.",
                        Status = TaskManager.Models.TaskStatus.NotStarted,
                        StartDate = DateTime.Now.AddDays(6),
                        EndDate = DateTime.Now.AddDays(12),
                        AssignedUserId = "80cd74b1-da1e-48e7-a79a-cdd3cad6e7a8",
                        ProjectId = mobileProject.Id
                    }
                );
                context.SaveChanges();
            }
                        
        }
        
    }

}

