﻿using FluentAssertions;
using LazyEntityGraph.EntityFramework.Tests.Edmx;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFramework;
using System.Collections;
using Xunit;
using LazyEntityGraph.TestUtils;

namespace LazyEntityGraph.EntityFramework.Tests.Edmx
{
    public class ModelMetadataGeneratorTests
    {
        public static ModelMetadata GetMetadata()
        {
            return ModelMetadataGenerator.LoadFromEdmxContext<BlogModelContainer>("BlogModel");
        }

        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange
            var expected = new[]
            {
                typeof (Post), typeof (User), typeof (Tag), typeof (ContactDetails), typeof (Category), typeof(Story)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.EntityTypes.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void ConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                ExpectedConstraints.CreateManyToMany<Post, Tag>(p => p.Tags, t => t.Posts),
                ExpectedConstraints.CreateManyToMany<Tag, Post>(t => t.Posts, p => p.Tags),
                ExpectedConstraints.CreateOneToMany<User, Post>(u => u.Posts, p => p.Poster),
                ExpectedConstraints.CreateManyToOne<Post, User>(p => p.Poster, u => u.Posts),
                ExpectedConstraints.CreateOneToOne<User, ContactDetails>(u => u.ContactDetails, c => c.User),
                ExpectedConstraints.CreateOneToOne<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                ExpectedConstraints.CreateForeignKey<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                ExpectedConstraints.CreateForeignKey<ContactDetails, User, int>(c => c.User, c => c.UserId, u => u.Id)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }
    }
}
