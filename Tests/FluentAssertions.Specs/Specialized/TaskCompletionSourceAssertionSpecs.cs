﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions.Extensions;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Specs.Specialized
{
    public class TaskCompletionSourceAssertionSpecs
    {
        [Fact]
        public async Task When_TCS_completes_in_time_it_should_succeed()
        {
            // Arrange
            var subject = new TaskCompletionSource<bool>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => subject.Should(timer).CompleteWithin(1.Seconds());
            subject.SetResult(true);
            timer.Complete();

            // Assert
            await action.Should().NotThrow();
        }

        [Fact]
        public async Task When_TCS_completes_in_time_and_result_is_expected_it_should_succeed()
        {
            // Arrange
            var subject = new TaskCompletionSource<int>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = async () => (await subject.Should(timer).CompleteWithin(1.Seconds())).Which.Should().Be(42);
            subject.SetResult(42);
            timer.Complete();

            // Assert
            await action.Should().NotThrow();
        }

        [Fact]
        public async Task When_TCS_completes_in_time_and_async_result_is_expected_it_should_succeed()
        {
            // Arrange
            var subject = new TaskCompletionSource<int>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => subject.Should(timer).CompleteWithin(1.Seconds()).WithResult(42);
            subject.SetResult(42);
            timer.Complete();

            // Assert
            await action.Should().NotThrow();
        }

        [Fact]
        public async Task When_TCS_completes_in_time_and_result_is_not_expected_it_should_fail()
        {
            // Arrange
            var testSubject = new TaskCompletionSource<int>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = async () => (await testSubject.Should(timer).CompleteWithin(1.Seconds())).Which.Should().Be(42);
            testSubject.SetResult(99);
            timer.Complete();

            // Assert
            await action.Should().Throw<XunitException>()
                .WithMessage("Expected *testSubject* to be 42, but found 99.");
        }

        [Fact]
        public async Task When_TCS_completes_in_time_and_async_result_is_not_expected_it_should_fail()
        {
            // Arrange
            var testSubject = new TaskCompletionSource<int>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => testSubject.Should(timer).CompleteWithin(1.Seconds()).WithResult(42);
            testSubject.SetResult(99);
            timer.Complete();

            // Assert
            await action.Should().Throw<XunitException>()
                .WithMessage("Expected testSubject to be 42, but found 99.");
        }

        [Fact]
        public async Task When_TCS_did_not_complete_in_time_it_should_fail()
        {
            // Arrange
            var subject = new TaskCompletionSource<bool>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => subject.Should(timer).CompleteWithin(1.Seconds(), "test {0}", "testArg");
            timer.Complete();

            // Assert
            await action.Should().Throw<XunitException>()
                .WithMessage("Expected subject to complete within 1s because test testArg.");
        }

        [Fact]
        public async Task When_TCS_is_null_it_should_fail()
        {
            // Arrange
            TaskCompletionSource<bool> subject = null;

            // Act
            Func<Task> action = () => subject.Should().CompleteWithin(1.Seconds());

            // Assert
            await action.Should().Throw<XunitException>()
                .WithMessage("Expected subject to complete within 1s, but found <null>.");
        }

        [Fact]
        public async Task When_TCS_completes_in_time_and_it_is_not_expected_it_should_fail()
        {
            // Arrange
            var subject = new TaskCompletionSource<bool>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => subject.Should(timer).NotCompleteWithin(1.Seconds(), "test {0}", "testArg");
            subject.SetResult(true);
            timer.Complete();

            // Assert
            await action.Should().Throw<XunitException>().WithMessage("*to not complete within*because test testArg*");
        }

        [Fact]
        public async Task When_TCS_did_not_complete_in_time_and_it_is_not_expected_it_should_succeed()
        {
            // Arrange
            var subject = new TaskCompletionSource<bool>();
            var timer = new FakeClock();

            // Act
            Func<Task> action = () => subject.Should(timer).NotCompleteWithin(1.Seconds());
            timer.Complete();

            // Assert
            await action.Should().NotThrow();
        }

        [Fact]
        public async Task When_TCS_is_null_and_we_validate_to_not_complete_it_should_fail()
        {
            // Arrange
            TaskCompletionSource<bool> subject = null;

            // Act
            Func<Task> action = () => subject.Should().NotCompleteWithin(1.Seconds(), "test {0}", "testArg");

            // Assert
            await action.Should().Throw<XunitException>()
                .WithMessage("Expected subject to not complete within 1s because test testArg, but found <null>.");
        }
    }
}
